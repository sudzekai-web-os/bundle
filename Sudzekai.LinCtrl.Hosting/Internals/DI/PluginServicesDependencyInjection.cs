using Microsoft.Extensions.DependencyInjection;

namespace Sudzekai.LinCtrl.Hosting
{
    internal static class PluginServicesDependencyInjection
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddPluginServices()
            {
                services.AddLoadingServices();
                services.AddInitializtionServices();
                services.AddDisposingServices();

                services.AddSingleton<PluginLifetimeHost>();
                services.AddSingleton<PluginsLifetimeHost>();

                return services;
            }

            private IServiceCollection AddLoadingServices()
            {
                services.AddSingleton<IPluginLoader, PluginManifestLoader>();
                services.AddSingleton<IPluginLoader, PluginAssembliesLoader>();

                return services;
            }

            private IServiceCollection AddInitializtionServices()
            {
                services.AddSingleton<IPluginInitializer, PluginAssembliesInitializer>();
                
                return services;
            }

            private IServiceCollection AddDisposingServices()
            {
                return services;
            }
        }
    }
}
