using Microsoft.Extensions.Logging;

namespace Sudzekai.LinCtrl.Hosting 
{
    internal class PluginsLoadingHost(
        PluginsRegistry registry,
        PluginDirectoryLoader loader,
        ILogger<PluginsLoadingHost> logger)
    {
        public void Load(string directoryPath = "plugins")
        {
            if (!Path.IsPathFullyQualified(directoryPath))
                directoryPath = Path.Combine(AppContext.BaseDirectory, directoryPath);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                return;
            }

            var pluginDirectoryPaths = Directory.EnumerateDirectories(directoryPath);

            foreach (var pluginDirectoryPath in pluginDirectoryPaths)
            {
                try
                {
                    var plugin = loader.Load(pluginDirectoryPath);
                    registry.Add(plugin);
                }
                catch (Exception ex)
                {
                    logger.LogError("{message} Plugin skipped.", ex.Message);
                }
            }
        }
    }
}
