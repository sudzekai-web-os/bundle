using Microsoft.Extensions.Logging;

namespace Sudzekai.LinCtrl.Hosting 
{
    internal class PluginRequestHandlersDisposingHost(
        RequestHandlerUnloadingHost host,
        ILogger<PluginRequestHandlersDisposingHost> logger)
    {
        public void Dispose(PluginInfo info)
        {
            logger.LogInformation("Disposing request handlers of plugin {name}", info.Name);

            foreach (var assembly in info.Assemblies)
                host.Unload(assembly.GetHttpRequestHandlers());
        }
    }
}
