using Microsoft.Extensions.Logging;

namespace Sudzekai.LinCtrl.Hosting
{
    internal class PluginAssembliesInitializer(
        AssemblyLifetimeHost assemblyHost,
        ILogger<PluginAssembliesInitializer> logger) : IPluginInitializer
    {
        public void Initialize(PluginInfo pluginInfo)
        {
            var tag = pluginInfo.GetTag();

            logger.LogDebug("[{tag}] Starting assemblies initialization...", tag);

            int c = 0;

            foreach (var assembly in pluginInfo.Assemblies)
            {
                try
                {
                    assemblyHost.Initialize(assembly);
                    c++;
                }
                catch (Exception ex)
                {
                    logger.LogError("[{tag}] Exception while initializing assembly: {message}. Assembly skipped", assembly.GetTag(), ex.Message);
                }
            }

            logger.LogDebug("[{tag}] Assemblies initialized (count: {count})", tag, c);

        }
    }
}
