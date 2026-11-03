using Microsoft.Extensions.Logging;

namespace Sudzekai.LinCtrl.Hosting 
{
    internal class PluginsLifetimeHost(
        PluginDisposingHost disposingHost,
        PluginInitializationHost initializationHost,
        PluginsRegistry registry,
        PluginRequestHandlersLifetimeHost requestHandlersLifetimeHost,
        ILogger<PluginsLifetimeHost> logger)
    {
        public void InitializePlugins()
        {
            logger.LogInformation("Initializing plugins...");

            foreach (var plugin in registry.GetList())
            {
                initializationHost.Initialize(plugin);
            }

            requestHandlersLifetimeHost.InitilizeHandlers();

            logger.LogInformation("Plugins initialized");
        }

        public void UnloadPlugins()
        {
            logger.LogInformation("Unloading plugins...");

            requestHandlersLifetimeHost.UnloadHandlers();

            foreach (var plugin in registry.GetList())
            {
                disposingHost.Dispose(plugin);

                registry.Remove(plugin);
            }

            logger.LogInformation("Plugins unloaded");
        }
    }
}
