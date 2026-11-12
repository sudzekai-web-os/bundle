using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.OpenApi;
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Hosting
{
    public static class WebApplicationBuilderExtensions
    {
        extension(WebApplicationBuilder builder)
        {
            public void AddLinCtrlInfrastructure()
            {
                LinCtrlContext.AddTypeInfoResolver(HostingContext.Default);
                builder.Services.ConfigureHttpJsonOptions(options =>
                    options.SerializerOptions.TypeInfoResolverChain.Insert(0, HostingContext.Default));
                builder.Services.ConfigureAll<OpenApiOptions>(options =>
                    options.AddDocumentTransformer<RequestHandlerOpenApiDocumentTransformer>());
                builder.Services.AddAssemblyServices();
                builder.Services.RegisterRequestHandlerServices();
                builder.Services.AddPluginServices();

                if (builder.Configuration["LinCtrlDatabase:Host"] != null || builder.Configuration["Jwt:Secret"] != null)
                    builder.Services.AddLinCtrlAuthentication(builder.Configuration);
            }
        }
    }
}
