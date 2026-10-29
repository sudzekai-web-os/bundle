using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Reader;

namespace Sudzekai.WebOs.Hosting
{
    internal sealed class PluginOpenApiDocumentTransformer(AssembliesRegistry registry) : IOpenApiDocumentTransformer
    {
        public Task TransformAsync(
            OpenApiDocument document,
            OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {
            foreach (var plugin in registry.GetPluginsList())
            {
                if (string.IsNullOrWhiteSpace(plugin.HandlersMetadataJson))
                    continue;

                var fragment = OpenApiDocument.Parse(plugin.HandlersMetadataJson, "json", new OpenApiReaderSettings()).Document;

                if (fragment?.Paths is not null)
                    MergePaths(document, fragment);

                if (fragment?.Tags is not null)
                    MergeTags(document, fragment);
            }

            return Task.CompletedTask;
        }

        private static void MergePaths(OpenApiDocument document, OpenApiDocument fragment)
        {
            document.Paths ??= [];

            foreach (var (path, fragmentItem) in fragment.Paths!)
            {
                if (!document.Paths.TryGetValue(path, out var targetItem))
                {
                    if (fragmentItem is not null)
                        document.Paths[path] = fragmentItem;
                    
                    continue;
                }

                if (targetItem is not null && targetItem.Operations is not null)
                {
                    foreach (var (method, operation) in fragmentItem.Operations ?? [])
                        targetItem.Operations[method] = operation;
                }
            }
        }

        private static void MergeTags(OpenApiDocument document, OpenApiDocument fragment)
        {
            document.Tags ??= new HashSet<OpenApiTag>();

            foreach (var tag in fragment.Tags!)
                document.Tags.Add(new OpenApiTag { Name = tag.Name });
        }
    }
}
