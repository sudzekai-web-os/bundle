using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Sudzekai.WebOs.Hosting
{
    public static class WebApplicationHostExtensions
    {
        extension (WebApplication app)
        {
            public async Task LoadPluginsAsync()
            {
                var host = app.Services.GetRequiredService<PluginHost>();

                await host.LoadPluginsAsync(default);
            }

            public void AddHandlers()
            {
                var registry = app.Services.GetRequiredService<PluginsRegistry>();
                var registerer = app.Services.GetRequiredService<PluginHandlersRegisterer>();

                var plugins = registry.GetPluginsList();

                foreach (var plugin in plugins)
                    registerer.RegisterHandlers(app, plugin);
            }

            public void UnloadPlugins()
            {
                var registry = app.Services.GetRequiredService<PluginsRegistry>();
                var registerer = app.Services.GetRequiredService<PluginHandlersRegisterer>();
                
                registry.UnloadPlugins();
            }

            public IReadOnlyList<Plugin> GetPluginsList()
            {
                var registry = app.Services.GetRequiredService<PluginsRegistry>();

                return registry.GetPluginsList();
            }
        }
    }
}
