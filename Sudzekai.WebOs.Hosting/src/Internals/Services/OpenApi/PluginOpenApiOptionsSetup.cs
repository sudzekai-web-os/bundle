using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;

namespace Sudzekai.WebOs.Hosting
{
    internal sealed class PluginOpenApiOptionsSetup(AssembliesRegistry registry) : IConfigureNamedOptions<OpenApiOptions>
    {
        public void Configure(string? name, OpenApiOptions options)
            => options.AddDocumentTransformer(new PluginOpenApiDocumentTransformer(registry));

        public void Configure(OpenApiOptions options)
            => Configure(Options.DefaultName, options);
    }
}
