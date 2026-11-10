using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Abstractions;
using Sudzekai.LinCtrl.Abi;
using Sudzekai.LinCtrl.Abi.Allocation;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Abi.Mapping;
using Sudzekai.LinCtrl.Assembly;
using Sudzekai.LinCtrl.Assembly.Context;
using Sudzekai.LinCtrl.Core;
using Sudzekai.LinCtrl.Hosting;
using System.Runtime.CompilerServices;

namespace TEST.Sudzekai.LinCtrl.Core
{
    public class ResourceLifetimeTests
    {
        [Fact]
        public unsafe void HostCachesHandlersAndDisposesScopedServicesOnce()
        {
            var descriptor = HostVTableAllocator.Alloc(LinCtrlContext.AbiVersion);
            var builder = AssemblyHost.CreateBuilder(new AssemblyContext(&descriptor));
            var counter = new DisposalCounter();
            builder.Services.AddLogging();
            builder.Services.AddSingleton(counter);
            builder.Services.AddScoped<IRequestHandler, ScopedRequestHandler>();
            var host = builder.Build();
            try
            {
                var handler = Assert.Single(host.GetHandlers());
                for (int i = 0; i < 100; i++)
                    Assert.Same(handler, Assert.Single(host.GetHandlers()));
                Assert.Throws<InvalidOperationException>(() => builder.Build());
                Assert.Equal(0, counter.Count);
            }
            finally
            {
                host.Dispose();
            }

            host.Dispose();
            Assert.Equal(1, counter.Count);
            Assert.Throws<ObjectDisposedException>(() => host.GetHandlers());
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public unsafe void AsyncOnlyScopedResourceIsReleasedOnDisposeOrResolutionFailure(bool fail)
        {
            var descriptor = HostVTableAllocator.Alloc(LinCtrlContext.AbiVersion);
            var builder = AssemblyHost.CreateBuilder(new AssemblyContext(&descriptor));
            var counter = new DisposalCounter();
            builder.Services.AddLogging();
            builder.Services.AddSingleton(counter);
            builder.Services.AddScoped<AsyncResource>();
            builder.Services.AddScoped<IRequestHandler>(provider =>
            {
                provider.GetRequiredService<AsyncResource>();
                if (fail)
                    throw new InvalidOperationException("Handler creation failed");
                return new EmptyRequestHandler();
            });
            var host = builder.Build();
            try
            {
                if (fail)
                {
                    Assert.Throws<InvalidOperationException>(() => host.GetHandlers());
                    Assert.Equal(1, counter.Count);
                }
                else
                {
                    Assert.Single(host.GetHandlers());
                    Assert.Equal(0, counter.Count);
                }
            }
            finally
            {
                host.Dispose();
            }

            Assert.Equal(1, counter.Count);
        }

        [Fact]
        public void RegistrationReceivesHandlersOnceAndReleasesThemOnDispose()
        {
            var (table, registration, handler, count) = CreateRegistration();
            for (int i = 0; i < 100; i++)
                Assert.Single(table.GetRequestHandlersDelegate());
            Assert.Equal(1, count.Count);

            table.AssemblyDisposeDelegate();
            table.AssemblyDisposeDelegate();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Assert.False(handler.IsAlive);
            Assert.True(registration.IsDisposed);
            Assert.Throws<ObjectDisposedException>(() => table.GetRequestHandlersDelegate());
            GC.KeepAlive(registration);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static (AssemblyVTable, AssemblyVTableRegistration, WeakReference, DisposalCounter) CreateRegistration()
        {
            var handler = new EmptyRequestHandler();
            var count = new DisposalCounter();
            var registration = new AssemblyVTableRegistration(new()
            {
                GetRequestHandlersDelegate = () =>
                {
                    count.Count++;
                    return [handler];
                },
                AssemblyDisposeDelegate = () => { }
            });
            return (AssemblyVTableMapper.ToManaged(registration.ToDescriptor()), registration, new(handler), count);
        }

        [Fact]
        public async Task DisposingWaitsForRequestsAndRejectsStaleHandlers()
        {
            using var entered = new ManualResetEventSlim();
            using var release = new ManualResetEventSlim();
            using var disposing = new ManualResetEventSlim();
            using var disposed = new ManualResetEventSlim();
            var registration = new AssemblyVTableRegistration(new()
            {
                GetRequestHandlersDelegate = () => [new BlockingRequestHandler(entered, release)],
                AssemblyDisposeDelegate = disposed.Set
            });
            var table = AssemblyVTableMapper.ToManaged(registration.ToDescriptor());
            var handler = Assert.Single(table.GetRequestHandlersDelegate());
            var token = TestContext.Current.CancellationToken;
            var request = Task.Run(() => handler.Handle(new()), token);
            Task? disposal = null;
            try
            {
                Assert.True(entered.Wait(TimeSpan.FromSeconds(5), token));
                disposal = Task.Run(() =>
                {
                    disposing.Set();
                    table.AssemblyDisposeDelegate();
                }, token);
                Assert.True(disposing.Wait(TimeSpan.FromSeconds(5), token));
                Assert.False(disposed.Wait(TimeSpan.FromMilliseconds(100), token));
            }
            finally
            {
                release.Set();
                await request.WaitAsync(TimeSpan.FromSeconds(5), token);
                if (disposal != null)
                    await disposal.WaitAsync(TimeSpan.FromSeconds(5), token);
                table.AssemblyDisposeDelegate();
            }

            Assert.True(disposed.IsSet);
            Assert.Throws<ObjectDisposedException>(() => handler.Handle(new()));
            GC.KeepAlive(registration);
        }

        [Fact]
        public void FailedInitializationReleasesAssemblyAndRemovesRoutes()
        {
            using var dataSource = new RequestHandlerEndpointDataSource();
            var counter = new DisposalCounter();
            var handler = new EmptyRequestHandler();
            var host = CreateAssemblyHost(dataSource, counter,
                [new CallbackInitializer(info =>
                {
                    info.Handlers = [handler];
                    dataSource.Add(handler);
                    throw new InvalidOperationException("Initialization failed");
                })]);
            var path = Path.GetTempFileName();
            try
            {
                var info = host.Load(path);
                Assert.Throws<InvalidOperationException>(() => host.Initialize(info));
                Assert.Equal(AssemblyLifetime.Disposed, info.Lifetime);
                Assert.Equal(1, counter.Count);
                Assert.Empty(dataSource.Endpoints);
                host.Dispose();
                Assert.Equal(1, counter.Count);
            }
            finally
            {
                host.Dispose();
                File.Delete(path);
            }
        }

        [Fact]
        public void ShutdownReleasesEveryAssemblyEvenIfOneDisposerThrows()
        {
            using var dataSource = new RequestHandlerEndpointDataSource();
            var counter = new DisposalCounter();
            var handlers = new RequestHandlerLifetimeHost(NullLogger<RequestHandlerLifetimeHost>.Instance, [], dataSource);
            var host = new AssemblyLifetimeHost(
                [new CallbackLoader(info => info.VTable = new()
                {
                    AssemblyDisposeDelegate = () =>
                    {
                        counter.Count++;
                        if (counter.Count == 1)
                            throw new InvalidOperationException("Disposal failed");
                    }
                })], [], NullLogger<AssemblyLifetimeHost>.Instance, handlers);
            var path = Path.GetTempFileName();
            try
            {
                var first = host.Load(path);
                var second = host.Load(path);
                Assert.Throws<AggregateException>(() => host.Dispose());
                Assert.Equal(2, counter.Count);
                Assert.Equal(AssemblyLifetime.Disposed, first.Lifetime);
                Assert.Equal(AssemblyLifetime.Disposed, second.Lifetime);
                host.Dispose();
            }
            finally
            {
                host.Dispose();
                File.Delete(path);
            }
        }

        [Fact]
        public void RemovingHandlerInvalidatesEndpointsAndDropsRegistration()
        {
            using var dataSource = new RequestHandlerEndpointDataSource();
            var handler = new EmptyRequestHandler();
            var token = dataSource.GetChangeToken();
            dataSource.Add(handler);
            Assert.True(token.HasChanged);
            Assert.Single(dataSource.Endpoints);
            token = dataSource.GetChangeToken();
            dataSource.Remove([handler]);
            Assert.True(token.HasChanged);
            Assert.Empty(dataSource.Endpoints);
        }

        [Fact]
        public void FailedLoaderReleasesPreviouslyExchangedTable()
        {
            using var dataSource = new RequestHandlerEndpointDataSource();
            var counter = new DisposalCounter();
            var handlers = new RequestHandlerLifetimeHost(NullLogger<RequestHandlerLifetimeHost>.Instance, [], dataSource);
            var host = new AssemblyLifetimeHost(
                [new CallbackLoader(info => info.VTable = new() { AssemblyDisposeDelegate = () => counter.Count++ }),
                 new CallbackLoader(_ => throw new InvalidOperationException("Load failed"))],
                [], NullLogger<AssemblyLifetimeHost>.Instance, handlers);
            var path = Path.GetTempFileName();
            try
            {
                Assert.Throws<InvalidOperationException>(() => host.Load(path));
                Assert.Equal(1, counter.Count);
                host.Dispose();
                Assert.Equal(1, counter.Count);
            }
            finally
            {
                host.Dispose();
                File.Delete(path);
            }
        }

        [Fact]
        public void ServiceProviderDisposalReleasesLoadedPlugins()
        {
            LinCtrlContext.AddTypeInfoResolver(HostingContext.Default);
            var services = new ServiceCollection();
            services.AddLogging();
            services.RegisterRequestHandlerServices();
            services.AddAssemblyServices();
            services.AddPluginServices();
            services.RemoveAll<IAssemblyLoader>();
            var counter = new DisposalCounter();
            services.AddSingleton<IAssemblyLoader>(new CallbackLoader(info => info.VTable = new()
            {
                InitDelegate = () => { },
                DelegateLoggingDelegate = _ => { },
                BuildDelegate = () => { },
                GetRequestHandlersDelegate = () => [],
                AssemblyDisposeDelegate = () => counter.Count++
            }));
            var provider = services.BuildServiceProvider();
            var directory = Directory.CreateTempSubdirectory("LinCtrlTests-");
            PluginInfo? plugin = null;
            try
            {
                File.WriteAllText(Path.Combine(directory.FullName, "manifest.json"),
                    "{ \"name\": \"test\", \"assemblies\": [\"module.dll\"] }");
                File.WriteAllBytes(Path.Combine(directory.FullName, "module.dll"), []);
                var host = provider.GetRequiredService<PluginLifetimeHost>();
                plugin = host.Load(directory.FullName);
                host.Initialize(plugin);
                Assert.Equal(PluginLifetime.Initialized, plugin.Lifetime);
                Assert.Equal(0, counter.Count);
            }
            finally
            {
                try
                {
                    provider.Dispose();
                }
                finally
                {
                    directory.Delete(true);
                }
            }

            Assert.NotNull(plugin);
            Assert.Equal(PluginLifetime.Disposed, plugin.Lifetime);
            Assert.Equal(1, counter.Count);
            provider.Dispose();
            Assert.Equal(1, counter.Count);
        }

        private static AssemblyLifetimeHost CreateAssemblyHost(
            RequestHandlerEndpointDataSource dataSource,
            DisposalCounter counter,
            IAssemblyInitializer[] initializers)
            => new([new CallbackLoader(info => info.VTable = new() { AssemblyDisposeDelegate = () => counter.Count++ })],
                initializers, NullLogger<AssemblyLifetimeHost>.Instance,
                new RequestHandlerLifetimeHost(NullLogger<RequestHandlerLifetimeHost>.Instance, [], dataSource));

        private sealed class DisposalCounter
        {
            public int Count;
        }

        private sealed class AsyncResource(DisposalCounter counter) : IAsyncDisposable
        {
            public ValueTask DisposeAsync()
                => new(Task.Run(() => Interlocked.Increment(ref counter.Count)));
        }

        private sealed class ScopedRequestHandler(DisposalCounter counter) : IRequestHandler, IDisposable
        {
            public string Pattern => "/scoped";

            public HttpRequestMethod HttpMethod => HttpRequestMethod.Get;

            public HttpResponseDto Handle(HttpRequestDto request)
                => new();

            public void Dispose()
                => counter.Count++;
        }

        private sealed class EmptyRequestHandler : IRequestHandler
        {
            public string Pattern => "/empty";

            public HttpRequestMethod HttpMethod => HttpRequestMethod.Get;

            public HttpResponseDto Handle(HttpRequestDto request)
                => new();
        }

        private sealed class BlockingRequestHandler(
            ManualResetEventSlim entered,
            ManualResetEventSlim release) : IRequestHandler
        {
            public string Pattern => "/blocking";

            public HttpRequestMethod HttpMethod => HttpRequestMethod.Get;

            public HttpResponseDto Handle(HttpRequestDto request)
            {
                entered.Set();
                if (!release.Wait(TimeSpan.FromSeconds(5)))
                    throw new TimeoutException();
                return new();
            }
        }

        private sealed class CallbackLoader(Action<AssemblyInfo> load) : IAssemblyLoader
        {
            public void Load(AssemblyInfo info)
                => load(info);
        }

        private sealed class CallbackInitializer(Action<AssemblyInfo> initialize) : IAssemblyInitializer
        {
            public void Initialize(AssemblyInfo info)
                => initialize(info);
        }
    }
}
