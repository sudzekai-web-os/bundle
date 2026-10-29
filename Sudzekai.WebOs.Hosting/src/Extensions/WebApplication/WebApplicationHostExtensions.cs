using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Sudzekai.WebOs.Core;

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

            public void MapHandlers()
            {
                var registry = app.Services.GetRequiredService<AssembliesRegistry>();
                var registerer = new AssemblyHandlersRegisterer(app);

                var plugins = registry.GetPluginsList();

                foreach (var plugin in plugins)
                    registerer.RegisterHandlers(plugin);

                using var scope = app.Services.CreateScope();

                var internalHandlers = scope.ServiceProvider.GetServices<IEndpointHandler>();

                foreach (var handler in internalHandlers) 
                    registerer.RegisterHandler(handler, "Internal");
            }

            public IReadOnlyList<PluginAssembly> GetPluginsList()
            {
                var registry = app.Services.GetRequiredService<AssembliesRegistry>();

                return registry.GetPluginsList();
            }
        }
    }
}
