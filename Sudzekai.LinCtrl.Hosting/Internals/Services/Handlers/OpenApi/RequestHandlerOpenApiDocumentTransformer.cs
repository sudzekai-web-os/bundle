using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Reader;
using Sudzekai.LinCtrl.Core;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Sudzekai.LinCtrl.Hosting
{
    internal sealed class RequestHandlerOpenApiDocumentTransformer(
        RequestHandlerEndpointDataSource endpoints,
        ILogger<RequestHandlerOpenApiDocumentTransformer> logger) : IOpenApiDocumentTransformer
    {
        public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {
            foreach (var (pattern, method, description) in endpoints.GetOpenApiOperations())
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    if (JsonNode.Parse(description) is not JsonObject node)
                        continue;

                    var group = node["x-linctrl-document"]?.GetValue<string>() ?? "v1";
                    if (!string.Equals(group, context.DocumentName, StringComparison.Ordinal))
                        continue;

                    node.Remove("x-linctrl-document");
                    if (node["tags"] is JsonArray tags)
                    {
                        document.Tags ??= new HashSet<OpenApiTag>();
                        foreach (var value in tags)
                        {
                            var name = value?.GetValue<string>();
                            if (!string.IsNullOrWhiteSpace(name) && !document.Tags.Any(tag => tag.Name == name))
                                document.Tags.Add(new OpenApiTag { Name = name });
                        }
                    }

                    var operation = new OpenApiJsonReader().ReadFragment<OpenApiOperation>(
                        node, OpenApiSpecVersion.OpenApi3_1, document, out var diagnostic);
                    if (operation == null || diagnostic.Errors.Count != 0)
                    {
                        logger.LogWarning("Invalid OpenAPI operation for route {pattern}", pattern);
                        continue;
                    }

                    var path = OpenApiMetadataWriter.NormalizePath(pattern);
                    document.Paths ??= new OpenApiPaths();
                    if (!document.Paths.TryGetValue(path, out var item) || item is not OpenApiPathItem pathItem)
                    {
                        pathItem = new OpenApiPathItem();
                        document.Paths[path] = pathItem;
                    }
                    pathItem.AddOperation(new HttpMethod(method), operation);
                }
                catch (JsonException ex)
                {
                    logger.LogWarning(ex, "Invalid OpenAPI operation for route {pattern}", pattern);
                }
            }

            return Task.CompletedTask;
        }
    }
}
