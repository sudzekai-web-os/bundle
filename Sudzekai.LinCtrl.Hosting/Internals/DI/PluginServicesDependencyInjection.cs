using Microsoft.Extensions.DependencyInjection;

namespace Sudzekai.LinCtrl.Hosting 
{
    internal static class PluginServicesDependencyInjection
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddPluginServices()
            {
                services.AddPluginLifetimeServices();
                services.AddPluginManifestValidator();
                services.AddPluginLoaders();

                return services;
            }

            private IServiceCollection AddPluginManifestValidator()
            {
                services.AddSingleton<IPluginManifestFieldValidator, PluginManifestVersionValidator>();
                services.AddSingleton<IPluginManifestFieldValidator, PluginManifestAssembilesValidator>();

                services.AddSingleton<PluginManifestValidator>();

                return services;
            }

            private IServiceCollection AddPluginLoaders()
            {
                services.AddSingleton<PluginManifestLoader>();

                services.AddSingleton<PluginDirectoryLoader>();

                services.AddSingleton<PluginsLoadingHost>();
                
                return services;
            }

            private IServiceCollection AddPluginLifetimeServices()
            {
                services.AddSingleton<PluginsRegistry>();

                services.AddSingleton<PluginDisposingHost>();
                services.AddSingleton<PluginInitializationHost>();

                services.AddSingleton<PluginsLifetimeHost>();

                return services;
            }
        }
    }
}
