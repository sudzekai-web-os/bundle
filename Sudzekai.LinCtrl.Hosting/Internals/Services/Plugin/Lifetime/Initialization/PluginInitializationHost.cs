using Microsoft.Extensions.Logging;

namespace Sudzekai.LinCtrl.Hosting 
{
    internal class PluginInitializationHost(
        AssemblyInitializationHost host,
        ILogger<PluginInitializationHost> logger)
    {
        public void Initialize(PluginInfo info)
        {
            var name = info.Name;
            logger.LogInformation("Initializing plugin {name}", name);

            foreach (var assembly in info.Assemblies)
                host.Initialize(assembly);

            info.IsInitialized = true;

            logger.LogInformation("Plugin {name} initialized", name);
        }
    }
}
