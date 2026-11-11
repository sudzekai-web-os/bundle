using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
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
        [Fact]
        public async Task FilteredEndpointsWriteEnvelopesAndExposeResponseMetadata()
        {
            var cancellationToken = TestContext.Current.CancellationToken;
            var registration = new AssemblyVTableRegistration(new()
            {
                GetRequestHandlersDelegate = () =>
                [
                    new ResponseHandler("/success", true, new HttpResponseDto
                    {
                        StatusCode = HttpStatusCode.OK,
                        ContentType = ResponseContentType.ApplicationJson,
                        Data = """{"value":42}"""u8.ToArray()
                    }),
                    new ResponseHandler("/error", true, HttpResponseDto.CreateError(HttpStatusCode.BadRequest, 123, "Rejected")),
                    new ResponseHandler("/raw", false, HttpResponseDto.CreateText(HttpStatusCode.OK, "raw")),
                    new ResponseHandler("/hidden-error-data", true,
                        HttpResponseDto.CreateText(HttpStatusCode.BadRequest, "internal details"))
                ],
                AssemblyDisposeDelegate = () => { }
            });
            var table = AssemblyVTableMapper.ToManaged(registration.ToDescriptor());
            try
            {
                var builder = WebApplication.CreateSlimBuilder();
                builder.AddLinCtrlInfrastructure();
                builder.Services.AddOpenApi();

                await using var app = builder.Build();
                app.MapLinCtrlInfrastructure();
                app.MapOpenApi();
                app.Urls.Add("http://127.0.0.1:0");
                var lifetime = app.Services.GetRequiredService<RequestHandlerLifetimeHost>();
                foreach (var handler in table.GetRequestHandlersDelegate())
                    lifetime.Build(handler);

                await app.StartAsync(cancellationToken);
                using var client = new HttpClient { BaseAddress = new Uri(app.Urls.Single()) };

                using var success = JsonDocument.Parse(await client.GetStringAsync("/success", cancellationToken));
                Assert.True(success.RootElement.GetProperty("success").GetBoolean());
                Assert.Equal(42, success.RootElement.GetProperty("data").GetProperty("value").GetInt32());

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
                using var fallback = JsonDocument.Parse(await hiddenError.Content.ReadAsStringAsync(cancellationToken));
                Assert.Equal("Bad Request", fallback.RootElement.GetProperty("error").GetProperty("message").GetString());

                var descriptions = app.Services.GetRequiredService<IApiDescriptionGroupCollectionProvider>()
                    .ApiDescriptionGroups.Items.SelectMany(group => group.Items);
                var responses = Assert.Single(descriptions, description => description.RelativePath == "success").SupportedResponseTypes;
                Assert.Equal(typeof(SuccessResponseEnvelope), Assert.Single(responses, type => type.StatusCode == 200).Type);
                Assert.Equal(typeof(ErrorResponseEnvelope), Assert.Single(responses, type => type.StatusCode == 400).Type);
                Assert.Equal(typeof(ErrorResponseEnvelope), Assert.Single(responses, type => type.StatusCode == 500).Type);
                using var openApi = JsonDocument.Parse(await client.GetStringAsync("/openapi/v1.json", cancellationToken));
                var documentedResponses = openApi.RootElement.GetProperty("paths").GetProperty("/success")
                    .GetProperty("get").GetProperty("responses");
                var successSchema = GetSchemaProperties(openApi.RootElement, documentedResponses, "200");
                Assert.True(successSchema.TryGetProperty("data", out _));
                Assert.False(successSchema.TryGetProperty("error", out _));
                var errorSchema = GetSchemaProperties(openApi.RootElement, documentedResponses, "400");
                Assert.True(errorSchema.TryGetProperty("error", out _));
                Assert.False(errorSchema.TryGetProperty("data", out _));
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

        private sealed class ResponseHandler(string pattern, bool isFiltering, HttpResponseDto response) : IRequestHandler
        {
            public string Pattern => pattern;
            public HttpRequestMethod HttpMethod => HttpRequestMethod.Get;
            public bool IsFiltering => isFiltering;

            public HttpResponseDto Handle(HttpRequestDto request) => response;
        }
    }
}
