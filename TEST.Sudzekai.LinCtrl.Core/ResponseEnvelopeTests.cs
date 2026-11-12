using Sudzekai.LinCtrl.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging.Abstractions;
using Sudzekai.LinCtrl.Hosting.Internals.Services.Assembly.Initialization;
using Microsoft.Extensions.DependencyInjection;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Abi.Mapping;
using Sudzekai.LinCtrl.Assembly.Extensions;
using Sudzekai.LinCtrl.Core;
using Sudzekai.LinCtrl.Hosting;
using System.Net;
using System.Text.Json;

namespace TEST.Sudzekai.LinCtrl.Core
{
    public class ResponseEnvelopeTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task FilteredEndpointsWriteEnvelopesAndExposeResponseMetadata(bool isHostHandler)
        {
            var cancellationToken = TestContext.Current.CancellationToken;
            var builder = WebApplication.CreateSlimBuilder();
            builder.AddLinCtrlInfrastructure();
            builder.Services.AddOpenApi();

            IRequestHandler[] handlers =
            [
                    new ResponseHandler("/success", true, new HttpResponseDto
                    {
                        StatusCode = HttpStatusCode.OK,
                        ContentType = ResponseContentType.ApplicationJson,
                        Data = """{"name":"sample"}"""u8.ToArray()
                    }, new HandlerMetadata("test", "Тест", ["database"], "Успех")
                    {
                        RequestBody = new(LinCtrlContext.GetTypeInfo<LoginRequest>()),
                        Parameters = [new("limit", "query", HostingContext.Default.Int32, "Размер страницы")],
                        Responses = [new(200, HostingContext.Default.PluginManifest), new(400)]
                    }, HttpRequestMethod.Post),
                    new ResponseHandler("/items/{id:long}", true, HttpResponseDto.Create(HttpStatusCode.OK)),
                    new ResponseHandler("/items/{id:long}", true, HttpResponseDto.Create(HttpStatusCode.NoContent),
                        httpMethod: HttpRequestMethod.Delete),
                    new ResponseHandler("/error", true, HttpResponseDto.CreateError(HttpStatusCode.BadRequest, 123, "Rejected")),
                    new ResponseHandler("/raw", true, HttpResponseDto.CreateText(HttpStatusCode.OK, "raw"),
                        new HandlerMetadata
                        {
                            Responses = [new(200, contentType: ResponseContentType.TextPlain) { Schema = """{"type":"string"}""" }]
                        }),
                    new ResponseHandler("/unfiltered-json", false, HttpResponseDto.CreateError(HttpStatusCode.BadRequest, 123, "Rejected")),
                    new ResponseHandler("/hidden-error-data", true,
                        HttpResponseDto.CreateText(HttpStatusCode.BadRequest, "internal details"))
            ];
            var hostHandler = new ResponseHandler("/host-only", true, HttpResponseDto.CreateText(HttpStatusCode.OK, "host"));
            builder.Services.AddSingleton(new HostRequestHandlerRegistration(_ => hostHandler, _ => { }, 1024));
            if (isHostHandler)
            {
                foreach (var handler in handlers)
                    builder.Services.AddSingleton(new HostRequestHandlerRegistration(_ => handler, _ => { }, 1024));
            }

            var registration = new AssemblyVTableRegistration(new()
            {
                GetRequestHandlersDelegate = () => handlers,
                GetRequestHandlersMetadataDelegate = () => handlers.WriteOpenApiMetadata(),
                AssemblyDisposeDelegate = () => { }
            });
            var table = AssemblyVTableMapper.ToManaged(registration.ToDescriptor());
            try
            {
                await using var app = builder.Build();
                app.MapLinCtrlInfrastructure();
                app.MapOpenApi();
                app.Urls.Add("http://127.0.0.1:0");
                if (!isHostHandler)
                {
                    var lifetime = app.Services.GetRequiredService<RequestHandlerLifetimeHost>();
                    var assembly = new AssemblyInfo("test/plugin.dll") { VTable = table, Handlers = table.GetRequestHandlersDelegate() };
                    assembly.SetLoaded();
                    new AssemblyRequestHandlersConfigurator(lifetime, NullLogger<AssemblyRequestHandlersConfigurator>.Instance)
                        .Initialize(assembly);
                }

                await app.StartAsync(cancellationToken);
                using var client = new HttpClient { BaseAddress = new Uri(app.Urls.Single()) };

                using var successResponse = await client.PostAsync("/success", new StringContent("{}"), cancellationToken);
                using var success = JsonDocument.Parse(await successResponse.Content.ReadAsStringAsync(cancellationToken));
                Assert.True(success.RootElement.GetProperty("success").GetBoolean());
                Assert.Equal("sample", success.RootElement.GetProperty("data").GetProperty("name").GetString());

                using var response = await client.GetAsync("/error", cancellationToken);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                using var error = JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
                Assert.False(error.RootElement.GetProperty("success").GetBoolean());
                Assert.False(error.RootElement.TryGetProperty("data", out _));
                var businessError = error.RootElement.GetProperty("error");
                Assert.Equal(123, businessError.GetProperty("code").GetInt32());
                Assert.Equal("Rejected", businessError.GetProperty("message").GetString());
                Assert.Equal("raw", await client.GetStringAsync("/raw", cancellationToken));

                using var hiddenError = await client.GetAsync("/hidden-error-data", cancellationToken);
                Assert.Equal(HttpStatusCode.BadRequest, hiddenError.StatusCode);
                Assert.Equal("internal details", await hiddenError.Content.ReadAsStringAsync(cancellationToken));
                using var unfiltered = await client.GetAsync("/unfiltered-json", cancellationToken);
                using var rawJson = JsonDocument.Parse(await unfiltered.Content.ReadAsStringAsync(cancellationToken));
                Assert.Equal(123, rawJson.RootElement.GetProperty("code").GetInt32());
                Assert.False(rawJson.RootElement.TryGetProperty("success", out _));

                using var openApi = JsonDocument.Parse(await client.GetStringAsync("/openapi/v1.json", cancellationToken));
                var paths = openApi.RootElement.GetProperty("paths");
                Assert.True(paths.GetProperty("/host-only").TryGetProperty("get", out _));
                var item = paths.GetProperty("/items/{id}");
                foreach (var method in new[] { "get", "delete" })
                {
                    var parameter = Assert.Single(item.GetProperty(method).GetProperty("parameters").EnumerateArray());
                    Assert.Equal("id", parameter.GetProperty("name").GetString());
                    Assert.Equal("path", parameter.GetProperty("in").GetString());
                    Assert.True(parameter.GetProperty("required").GetBoolean());
                    Assert.Equal("integer", parameter.GetProperty("schema").GetProperty("type").GetString());
                }
                var rawContent = paths.GetProperty("/raw").GetProperty("get").GetProperty("responses").GetProperty("200").GetProperty("content");
                Assert.True(rawContent.TryGetProperty("text/plain", out _));
                Assert.False(rawContent.TryGetProperty("application/json", out _));
                var operation = paths.GetProperty("/success").GetProperty("post");
                Assert.Equal("database", Assert.Single(operation.GetProperty("tags").EnumerateArray()).GetString());
                var requestSchema = operation.GetProperty("requestBody").GetProperty("content")
                    .GetProperty("application/json").GetProperty("schema").GetProperty("properties");
                Assert.Equal("string", requestSchema.GetProperty("username").GetProperty("type").GetString());
                Assert.Equal("string", requestSchema.GetProperty("password").GetProperty("type").GetString());
                Assert.Equal("limit", Assert.Single(operation.GetProperty("parameters").EnumerateArray()).GetProperty("name").GetString());
                var documentedResponses = operation.GetProperty("responses");
                var successSchema = GetSchemaProperties(openApi.RootElement, documentedResponses, "200");
                var dataSchema = successSchema.GetProperty("data");
                Assert.Equal("object", dataSchema.GetProperty("type").GetString());
                Assert.Equal("string", dataSchema.GetProperty("properties").GetProperty("name").GetProperty("type").GetString());
                Assert.False(successSchema.TryGetProperty("error", out _));
                var errorSchema = GetSchemaProperties(openApi.RootElement, documentedResponses, "400");
                Assert.True(errorSchema.TryGetProperty("error", out _));
                Assert.False(errorSchema.TryGetProperty("data", out _));
                if (isHostHandler)
                {
                    using var oversized = await client.PostAsync("/success", new StringContent(new string('x', 1025)), cancellationToken);
                    Assert.Equal(HttpStatusCode.RequestEntityTooLarge, oversized.StatusCode);
                    using var envelope = JsonDocument.Parse(await oversized.Content.ReadAsStringAsync(cancellationToken));
                    Assert.False(envelope.RootElement.GetProperty("success").GetBoolean());
                    Assert.Equal(413, envelope.RootElement.GetProperty("error").GetProperty("code").GetInt32());
                }

                await app.StopAsync(cancellationToken);
            }
            finally
            {
                table.AssemblyDisposeDelegate();
                GC.KeepAlive(registration);
            }
        }

        private static JsonElement GetSchemaProperties(JsonElement document, JsonElement responses, string statusCode)
        {
            var schema = responses.GetProperty(statusCode).GetProperty("content")
                .GetProperty("application/json").GetProperty("schema");
            if (schema.TryGetProperty("$ref", out var reference))
            {
                var schemaName = reference.GetString()!.Split('/')[^1];
                schema = document.GetProperty("components").GetProperty("schemas").GetProperty(schemaName);
            }

            return schema.GetProperty("properties");
        }

        private sealed class ResponseHandler(string pattern, bool isFiltering, HttpResponseDto response, HandlerMetadata? openApiInfo = null,
            HttpRequestMethod httpMethod = HttpRequestMethod.Get) : IRequestHandler
        {
            public string Pattern => pattern;
            public HttpRequestMethod HttpMethod => httpMethod;
            public HandlerMetadata? Metadata => openApiInfo;
            public bool IsFiltered => isFiltering;

            public HttpResponseDto Handle(HttpRequestDto request) => response;
        }
    }
}
