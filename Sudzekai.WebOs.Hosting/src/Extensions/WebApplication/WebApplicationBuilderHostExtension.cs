using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Sudzekai.WebOs.Hosting
{
    public static class WebApplicationBuilderHostExtension
    {
        extension(WebApplicationBuilder builder)
        {
            public void AddPluginInfrastructure()
            {
                builder.Services.AddSingleton<IConfigureOptions<OpenApiOptions>, PluginOpenApiOptionsSetup>();

                // handlers configuration

                builder.Services.AddSingleton<HandlerConfigurator>();

                builder.Services.AddTransient<IHandlerConfigurationStepHandler, HandlerAuthorizationStepHandler>();
                builder.Services.AddTransient<IHandlerConfigurationStepHandler, HandlerLimitingStepHandler>();
                builder.Services.AddTransient<IHandlerConfigurationStepHandler, HandlerRoutingStepHandler>();

                // plugins loading

                builder.Services.AddTransient<AssemblyLoader>();

                builder.Services.AddTransient<IAssemblyLoadingStepHandler, AssemblyMainStepHandler>();                  // 1
                builder.Services.AddSingleton<IAssemblyLoadingStepHandler, AssemblyDelegateLoggingStepHandler>();       // 2
                builder.Services.AddTransient<IAssemblyLoadingStepHandler, AssemblyBuildStepHandler>();                 // 3
                builder.Services.AddTransient<IAssemblyLoadingStepHandler, AssemblyGetInformationStepHandler>();        // 4
                builder.Services.AddTransient<IAssemblyLoadingStepHandler, AssemblyGetHandlersStepHandler>();           // 5
                builder.Services.AddTransient<IAssemblyLoadingStepHandler, AssemblyGetHandlersMetadataStepHandler>();   // 6
                builder.Services.AddTransient<IAssemblyLoadingStepHandler, AssemblyGetDisposeDelegateStepHandler>();    // 7

                builder.Services.AddSingleton<AssembliesRegistry>();

                builder.Services.AddSingleton<PluginHost>();
            }
        }
    }
}
