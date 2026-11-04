using Microsoft.Extensions.Logging;

namespace Sudzekai.LinCtrl.Hosting
{
    internal class PluginLifetimeHost(
        IEnumerable<IPluginLoader> loaders,
        IEnumerable<IPluginInitializer> initializers,
        IEnumerable<IPluginDisposer> disposers,
        ILogger<PluginLifetimeHost> logger)
    {
        private HashSet<PluginInfo> _plugins = [];

        public PluginInfo Load(string directoryPath)
        {
            if (_plugins.FirstOrDefault(x => x.DirectoryPath == directoryPath) != null)
                throw new InvalidOperationException($"Plugin already loaded");

            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException($"Directory not found");

            PluginInfo plugin = new(directoryPath);
            var tag = plugin.GetTag();

            logger.LogInformation("[{tag}] Plugin loading started", tag);

            foreach (var loader in loaders)
                loader.Load(plugin);

            plugin.SetLoaded();

            _plugins.Add(plugin);

            logger.LogInformation("[{tag}] Plugin loaded", tag);
            
            return plugin;
        }

        public void Initialize(PluginInfo plugin)
        {
            if (plugin.Lifetime == PluginLifetime.Initialized)
                throw new InvalidOperationException($"Plugin already initialized");

            if (plugin.Lifetime < PluginLifetime.Loaded)
                throw new InvalidOperationException($"Plugin must be loaded before initialization");

            var tag = plugin.GetTag();

            logger.LogInformation("[{tag}] Plugin initialization started", tag);

            foreach (var initializer in initializers)
                initializer.Initialize(plugin);

            plugin.SetInitialized();

            logger.LogInformation("[{tag}] Plugin initialized", tag);

            _plugins.Add(plugin);
        }

        public void Dispose(PluginInfo plugin)
        {
            if (plugin.Lifetime == PluginLifetime.Disposed)
                throw new InvalidOperationException($"Plugin already disposed");

            if (plugin.Lifetime < PluginLifetime.Loaded)
                throw new InvalidOperationException($"Plugin must be loaded or initialized before disposing");

            var tag = plugin.GetTag();

            logger.LogInformation("[{tag}] Plugin disposing started", tag);

            foreach (var disposer in disposers)
                disposer.Dispose(plugin);

            plugin.SetDisposed();

            logger.LogInformation("[{tag}] Plugin disposed", tag);
            
            _plugins.Remove(plugin);
        }

        public IReadOnlyList<PluginInfo> GetPlugins()
            => [.. _plugins];

        public IReadOnlyList<PluginInfo> GetLoadedPlugins()
            => [.. _plugins.Where(p => p.Lifetime == PluginLifetime.Loaded)];

        public IReadOnlyList<PluginInfo> GetInitializedPlugins()
            => [.. _plugins.Where(p => p.Lifetime == PluginLifetime.Initialized)];
    }
}
