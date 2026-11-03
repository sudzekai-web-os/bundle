using Microsoft.Extensions.Logging;

namespace Sudzekai.LinCtrl.Hosting 
{
    internal class PluginRequestHandlersLifetimeHost(
        PluginRequestHandlersDisposingHost disposingHost,
        PluginRequestHandlersInitializationHost initializationHost,
        PluginsRegistry registry,
        ILogger<PluginRequestHandlersLifetimeHost> logger)
    {
        public void InitilizeHandlers()
        {
            logger.LogInformation("Initializing handlers...");

            foreach (var plugin in registry.GetList())
                initializationHost.Initialize(plugin);

            logger.LogInformation("Handlers initialized");
        }

        public void UnloadHandlers()
        {
            logger.LogInformation("Unloading handlers...");

            foreach (var plugin in registry.GetList())
            {
                disposingHost.Dispose(plugin);
            }

            logger.LogInformation("Handlers unloaded");
        }
    }
}
