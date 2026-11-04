using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Sudzekai.LinCtrl.Hosting
{
    public static class WebApplicationExtensions
    {
        extension(WebApplication application)
        {
            public void MapLinCtrlInfrastructure()
            {
            }

            public void LoadPlugins(string pluginsDirectoryPath = "plugins")
            {
                var pluginsLifetime = application.Services.GetRequiredService<PluginsLifetimeHost>();

                pluginsLifetime.Load(pluginsDirectoryPath);
            }

            public void InitializePlugins()
            {
                var pluginsLifetime = application.Services.GetRequiredService<PluginsLifetimeHost>();

                pluginsLifetime.Initialize(pluginsLifetime.GetLoadedPlugins());
            }

            public IReadOnlyList<PluginInfo> GetPlugins()
            {
                var pluginsLifetime = application.Services.GetRequiredService<PluginsLifetimeHost>();

                return pluginsLifetime.GetPlugins();
            }

            public void UnloadPlugins()
            {
                var pluginsLifetime = application.Services.GetRequiredService<PluginLifetimeHost>();

                foreach (var plugin in pluginsLifetime.GetPlugins())
                    pluginsLifetime.Dispose(plugin);
            }
        }
    }
}
