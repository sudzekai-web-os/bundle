using Microsoft.Extensions.Logging;

namespace Sudzekai.LinCtrl.Hosting
{
    internal class PluginDisposingHost(
        AssemblyDisposingHost host,
        ILogger<PluginDisposingHost> logger)
    {
        public void Dispose(PluginInfo info)
        {
            var name = info.Name;
            logger.LogInformation("Disposing plugin {name}", name);

            foreach (var assembly in info.Assemblies)
                host.Dispose(assembly);

            info.IsDisposed = true;

            logger.LogInformation("Plugin {name} disposed", name);
        }
    }
}
