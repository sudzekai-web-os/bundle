using Microsoft.Extensions.Logging;

namespace Sudzekai.LinCtrl.Hosting 
{
    internal class PluginsRegistry(ILogger<PluginsRegistry> logger)
    {
        private readonly List<PluginInfo> _plugins = [];

        public void Add(PluginInfo info)
        {
            _plugins.Add(info);
            logger.LogDebug("Plugin {name} added", info.Name);
        }

        public IReadOnlyList<PluginInfo> GetList()
            => _plugins;
        
        public IReadOnlyList<PluginInfo> GetInitialized()
            => [.._plugins.Where(p => p.IsInitialized)];

        public IReadOnlyList<PluginInfo> GetNotInitialized()
            => [.._plugins.Where(p => !p.IsInitialized)];

        public bool Remove(PluginInfo info)
        {
            if (_plugins.Remove(info))
            {
                logger.LogDebug("Plugin {name} removed", info.Name);
                return true;
            }

            return false;
        }
    }
}
