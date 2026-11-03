using Microsoft.Extensions.Logging;

namespace Sudzekai.LinCtrl.Hosting 
{
    internal class PluginRequestHandlersInitializationHost(
        RequestHandlerLoadingHost host,
        ILogger<PluginRequestHandlersInitializationHost> logger)
    {
        public void Initialize(PluginInfo info)
        {
            logger.LogInformation("Initializing request handlers of plugin {name}", info.Name);

            foreach (var assembly in info.Assemblies)
                host.Load(assembly.GetHttpRequestHandlers());
        }
    }
}
