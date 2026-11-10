using Microsoft.Extensions.Logging;

namespace Sudzekai.LinCtrl.Hosting
{
    internal class PluginsLifetimeHost(
        PluginLifetimeHost host,
        ILogger<PluginsLifetimeHost> logger)
    {
        public IEnumerable<PluginInfo> Load(string directoryPath)
        {
            var absolutePath = Path.Combine(AppContext.BaseDirectory, directoryPath);

            logger.LogInformation("Starting plugins loading from {directoryPath}...", absolutePath);

            if (!Directory.Exists(absolutePath))
            {
                Directory.CreateDirectory(absolutePath);
                return [];
            }

            foreach (var pluginDirectory in Directory.EnumerateDirectories(absolutePath))
            {
                try
                {
                    host.Load(pluginDirectory);
                }
                catch (Exception ex)
                {
                    var tag = Path.GetFileName(pluginDirectory);

                    logger.LogError("[{tag}] Exception while loading plugin: {message}. Plugin skipped", tag, ex.Message);
                }
            }

            logger.LogInformation("Plugins loaded");

            return GetLoadedPlugins();
        }

        public void Initialize(IEnumerable<PluginInfo> plugins)
        {
            logger.LogInformation("Starting plugins initialization...");
            
            foreach (var plugin in plugins)
                host.Initialize(plugin);

            logger.LogInformation("Plugins initialized");
        }

        public void Dispose(IEnumerable<PluginInfo> plugins)
        {
            logger.LogInformation("Starting plugins disposing...");

            List<Exception> exceptions = [];
            foreach (var plugin in plugins.ToArray())
            {
                try
                {
                    host.Dispose(plugin);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Count > 0)
                throw new AggregateException("Failed to release plugins", exceptions);

            logger.LogInformation("Plugins disposed");
        }

        public IReadOnlyList<PluginInfo> GetPlugins()
            => host.GetPlugins();

        public IReadOnlyList<PluginInfo> GetLoadedPlugins()
            => host.GetLoadedPlugins();

        public IReadOnlyList<PluginInfo> GetInitializedPlugins()
            => host.GetInitializedPlugins();
    }
}
