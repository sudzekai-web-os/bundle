using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sudzekai.LinCtrl.Assembly;
using Sudzekai.LinCtrl.Assembly.Context;
using Sudzekai.LinCtrl.Abi;
using Sudzekai.LinCtrl.Abi.Allocation;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Abi.Mapping;
using Sudzekai.LinCtrl.Core;
using System.Net;
using System.Text;

namespace TEST.Sudzekai.LinCtrl.Core
{
    public class AbiContractTests
    {
        [Fact]
        public unsafe void VirtualTableTransfersHandlersRequestsResponsesAndLogs()
        {
            bool initialized = false;
            bool built = false;
            bool disposed = false;
            string? message = null;
            var handler = new EchoRequestHandler();
            var table = new AssemblyVTable
            {
                InitDelegate = () => initialized = true,
                BuildDelegate = () => built = true,
                DelegateLoggingDelegate = log => log(new() { Category = "test", Message = "ready" }),
                GetRequestHandlersDelegate = () => [handler],
                GetRequestHandlersMetadataDelegate = () => "metadata",
                AssemblyDisposeDelegate = () => disposed = true
            };
            var registration = new AssemblyVTableRegistration(table);
            var native = registration.ToDescriptor();
            var managed = AssemblyVTableMapper.ToManaged(native);

            managed.InitDelegate();
            managed.DelegateLoggingDelegate(entry => message = entry.Message);
            managed.BuildDelegate();
            var handlers = managed.GetRequestHandlersDelegate();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            var response = Assert.Single(handlers).Handle(new()
            {
                HttpMethod = HttpRequestMethod.Post,
                Host = "localhost",
                Scheme = "http",
                Path = "/echo",
                Headers = new() { ["X-Test"] = ["first", "second"] },
                Claims = new() { ["role"] = ["user"] },
                RouteValues = new() { ["id"] = "42" },
                Cookies = new() { ["session"] = "value" },
                Body = Encoding.UTF8.GetBytes("Привет")
            });

            Assert.True(initialized);
            Assert.True(built);
            Assert.Equal("ready", message);
            Assert.Equal("/echo", handlers[0].Pattern);
            Assert.Equal(HttpRequestMethod.Post, handlers[0].HttpMethod);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Привет", Encoding.UTF8.GetString(response.Data));
            Assert.Equal("metadata", managed.GetRequestHandlersMetadataDelegate());
            managed.AssemblyDisposeDelegate();
            Assert.True(disposed);
            GC.KeepAlive(registration);
        }

        [Fact]
        public void NativeCallbackFailureBecomesManagedException()
        {
            var registration = new AssemblyVTableRegistration(new()
            {
                InitDelegate = () => throw new InvalidOperationException("Initialization failed")
            });
            var managed = AssemblyVTableMapper.ToManaged(registration.ToDescriptor());

            Assert.Throws<InvalidOperationException>(() => managed.InitDelegate());
            GC.KeepAlive(registration);
        }

        [Fact]
        public unsafe void EmptyRequestAndResponseDataRoundTrip()
        {
            var request = HttpDtoMapper.ToDescriptor(new HttpRequestDto());
            var response = HttpDtoMapper.ToDescriptor(new HttpResponseDto() { StatusCode = HttpStatusCode.NoContent });
            try
            {
                Assert.Empty(HttpDtoMapper.ToManaged(request).Body);
                Assert.Empty(HttpDtoMapper.ToManaged(request).Headers);
                Assert.Empty(HttpDtoMapper.ToManaged(response).Data);
                Assert.Equal(HttpStatusCode.NoContent, HttpDtoMapper.ToManaged(response).StatusCode);
            }
            finally
            {
                HttpRequestDtoAllocator.Free(ref request);
                HttpResponseDtoAllocator.Free(ref response);
            }
        }

        [Fact]
        public void GeneratedPluginEntryPointUsesNewContracts()
        {
            for (int i = 0; i < 3; i++)
                RunGeneratedPlugin();
        }

        private static unsafe void RunGeneratedPlugin()
        {
            var host = HostVTableAllocator.Alloc(LinCtrlContext.AbiVersion);
            AssemblyVTableDescriptor* descriptor = null;
            delegate* unmanaged[Cdecl]<HostVTableDescriptor*, AssemblyVTableDescriptor**, int> exchange
                = &LinCtrlAssembly.LinCtrlTestPlugin.Exchange;

            Assert.Equal(0, exchange(&host, &descriptor));
            Assert.True(descriptor != null);
            AssemblyVTable? table = null;
            try
            {
                table = AssemblyVTableMapper.ToManaged(*descriptor);
                table.InitDelegate();
                table.DelegateLoggingDelegate(_ => { });
                table.BuildDelegate();
                var handlers = table.GetRequestHandlersDelegate();
                var request = new HttpRequestDto { Host = "localhost", Scheme = "http", Path = "/object" };

                var handler = Assert.Single(handlers, h => h.Pattern == "object");
                Assert.True(handler.IsFiltered);
                var response = handler.Handle(request);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(ResponseContentType.ApplicationJson, response.ContentType);
                using var json = System.Text.Json.JsonDocument.Parse(response.Data);
                Assert.Equal(System.Text.Json.JsonValueKind.Object, json.RootElement.ValueKind);

                var error = Assert.Single(handlers, h => h.Pattern == "object/error").Handle(request);
                Assert.Equal(HttpStatusCode.InternalServerError, error.StatusCode);
            }
            finally
            {
                try
                {
                    table?.AssemblyDisposeDelegate();
                }
                finally
                {
                    CoTaskAllocator.Free(ref descriptor);
                }
            }
        }

        [Fact]
        public unsafe void PluginConfigurationSurvivesReleaseOfHostDescriptor()
        {
            var hostConfiguration = new ConfigurationBuilder().AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Storage:Paths:0"] = "/data",
                    ["Jwt:Issuer"] = "LinCtrl",
                    ["Empty"] = string.Empty,
                    ["ExplicitNull"] = null
                }).Build();
            var descriptor = HostVTableAllocator.Alloc(LinCtrlContext.AbiVersion, hostConfiguration.AsEnumerable());
            AssemblyContext context;
            try
            {
                context = new(&descriptor);
            }
            finally
            {
                HostVTableAllocator.Free(ref descriptor);
            }

            Assert.False(descriptor.IsAllocated);
            var builder = AssemblyHost.CreateBuilder(context);
            Assert.Equal("/data", builder.Configuration["storage:paths:0"]);
            Assert.Equal("LinCtrl", builder.Configuration["Jwt:Issuer"]);
            Assert.Equal(string.Empty, builder.Configuration["Empty"]);
            Assert.Contains(builder.Configuration.GetChildren(), child => child.Key == "ExplicitNull" && child.Value is null);

            using var host = builder.Build();
            Assert.Same(builder.Configuration, host.Services.GetRequiredService<IConfiguration>());
        }

        [Fact]
        public void UnsupportedAbiVersionIsRejected()
        {
            Assert.Throws<InvalidOperationException>(() => AssemblyVTableMapper.ToManaged(new() { AbiVersion = -1 }));
        }

        private sealed class EchoRequestHandler : IRequestHandler
        {
            public string Pattern => "/echo";

            public HttpRequestMethod HttpMethod => HttpRequestMethod.Post;

            public HttpResponseDto Handle(HttpRequestDto request)
            {
                Assert.Equal(["first", "second"], request.Headers["X-Test"]);
                Assert.Equal("user", Assert.Single(request.Claims["role"]));
                Assert.Equal("42", request.RouteValues["id"]);
                Assert.Equal("value", request.Cookies["session"]);
                Assert.Empty(request.QueryString);
                return new() { StatusCode = HttpStatusCode.OK, ContentType = ResponseContentType.TextPlain, Data = request.Body };
            }
        }
    }
}
