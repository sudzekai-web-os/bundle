using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Sudzekai.LinCtrl.Hosting 
{
    public static class WebApplicationExtensions
    {
        extension(WebApplication application)
        {
            public void MapLinCtrlInfrastructure()
            {
                application.Services.GetRequiredService<RequestHandlerLoader>().RouteBuilder = application;
                application.Services.GetRequiredService<RequestHandlerUnloader>().RouteBuilder = application;
            }

            public void LoadPlugins(string pluginsDirectoryPath = "plugins")
            {
                var lifetime = application.Services.GetRequiredService<IHostApplicationLifetime>();

                var pluginsLifetime = application.Services.GetRequiredService<PluginsLifetimeHost>();

                lifetime.ApplicationStopping.Register(pluginsLifetime.UnloadPlugins);

                application.Services.GetRequiredService<PluginsLoadingHost>().Load();
            }

            public IReadOnlyList<PluginInfo> GetPlugins()
            {
                var registry = application.Services.GetRequiredService<PluginsRegistry>();

                return registry.GetList();
            }

            public void UnloadPlugins()
            {
                var pluginsLifetime = application.Services.GetRequiredService<PluginsLifetimeHost>();

                pluginsLifetime.UnloadPlugins();
            }
        }
    }
}
