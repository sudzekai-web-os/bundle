using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
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
                builder.Services.TryAddEnumerable(ServiceDescriptor.Transient<IApiDescriptionProvider, PluginApiDescriptionProvider>());
                builder.Services.ConfigureAll<OpenApiOptions>(options =>
                    options.AddOperationTransformer<ResponseEnvelopeOperationTransformer>());
                builder.Services.AddAssemblyServices();
                builder.Services.RegisterRequestHandlerServices();
                builder.Services.AddPluginServices();

                if (builder.Configuration["LinCtrlDatabase:Host"] != null || builder.Configuration["Jwt:Secret"] != null)
                    builder.Services.AddLinCtrlAuthentication(builder.Configuration);
            }
        }
    }
}
