using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Reader;
using Sudzekai.LinCtrl.Abi.Managed;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Sudzekai.LinCtrl.Hosting
{
    internal sealed class ResponseEnvelopeOperationTransformer(ILogger<ResponseEnvelopeOperationTransformer> logger) : IOpenApiOperationTransformer
    {
        public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            var metadata = context.Description.ActionDescriptor.EndpointMetadata.OfType<RequestHandlerMetadata>().LastOrDefault();
            if (metadata?.ResponseDataSchema is not { Length: > 0 } schemaJson
                || operation.Responses == null || context.Document is not { } document)
                return Task.CompletedTask;

            var dataSchema = ReadSchema(schemaJson, document, metadata.Pattern);
            if (dataSchema == null)
                return Task.CompletedTask;

            var successTypes = context.Description.SupportedResponseTypes.Where(response => response.Type == typeof(SuccessResponseEnvelope));
            foreach (var response in successTypes)
            {
                if (!operation.Responses.TryGetValue(response.StatusCode.ToString(CultureInfo.InvariantCulture), out var result)
                    || result.Content == null || !result.Content.TryGetValue("application/json", out var content))
                    continue;

                content.Schema = new OpenApiSchema
                {
                    Type = JsonSchemaType.Object,
                    Required = new HashSet<string> { "success", "data" },
                    Properties = new Dictionary<string, IOpenApiSchema>
                    {
                        ["success"] = new OpenApiSchema { Type = JsonSchemaType.Boolean, Default = JsonValue.Create(true) },
                        ["data"] = dataSchema
                    }
                };
            }

            return Task.CompletedTask;
        }

        private OpenApiSchema? ReadSchema(string schemaJson, OpenApiDocument document, string pattern)
        {
            try
            {
                if (JsonNode.Parse(schemaJson) is JsonObject schemaNode)
                {
                    var schema = new OpenApiJsonReader().ReadFragment<OpenApiSchema>(
                        schemaNode, OpenApiSpecVersion.OpenApi3_1, document, out var diagnostic);
                    if (diagnostic.Errors.Count == 0)
                        return schema;
                }
            }
            catch (JsonException)
            {
            }

            logger.LogWarning("Invalid response data schema for endpoint {pattern}", pattern);
            return null;
        }
    }
}
