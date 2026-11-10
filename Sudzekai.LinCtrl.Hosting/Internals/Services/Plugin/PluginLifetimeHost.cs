using Microsoft.Extensions.Logging;

namespace Sudzekai.LinCtrl.Hosting
{
    internal class PluginLifetimeHost(
        IEnumerable<IPluginLoader> loaders,
        IEnumerable<IPluginInitializer> initializers,
        IEnumerable<IPluginDisposer> disposers,
        ILogger<PluginLifetimeHost> logger) : IDisposable
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

            try
            {
                foreach (var loader in loaders)
                    loader.Load(plugin);
            }
            catch
            {
                DisposeAfterFailure(plugin);
                throw;
            }

            plugin.SetLoaded();

            _plugins.Add(plugin);

            logger.LogInformation("[{tag}] Plugin loaded", tag);
            
            return plugin;
        }

        public void Initialize(PluginInfo plugin)
        {
            ObjectDisposedException.ThrowIf(plugin.Lifetime == PluginLifetime.Disposed, plugin);

            if (plugin.Lifetime == PluginLifetime.Initialized)
                throw new InvalidOperationException($"Plugin already initialized");

            if (plugin.Lifetime < PluginLifetime.Loaded)
                throw new InvalidOperationException($"Plugin must be loaded before initialization");

            var tag = plugin.GetTag();

            logger.LogInformation("[{tag}] Plugin initialization started", tag);

            try
            {
                foreach (var initializer in initializers)
                    initializer.Initialize(plugin);
            }
            catch
            {
                DisposeAfterFailure(plugin);
                throw;
            }

            plugin.SetInitialized();

            logger.LogInformation("[{tag}] Plugin initialized", tag);

            _plugins.Add(plugin);
        }

        public void Dispose(PluginInfo plugin)
        {
            if (plugin.Lifetime == PluginLifetime.Disposed)
                return;

            var tag = plugin.GetTag();

            logger.LogInformation("[{tag}] Plugin disposing started", tag);

            List<Exception> exceptions = [];
            try
            {
                foreach (var disposer in disposers)
                {
                    try
                    {
                        disposer.Dispose(plugin);
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }
            }
            finally
            {
                plugin.SetDisposed();
                _plugins.Remove(plugin);
            }

            logger.LogInformation("[{tag}] Plugin disposed", tag);

            if (exceptions.Count > 0)
                throw new AggregateException("Failed to release plugin resources", exceptions);
        }

        private void DisposeAfterFailure(PluginInfo plugin)
        {
            try
            {
                Dispose(plugin);
            }
            catch (Exception ex)
            {
                logger.LogError("Exception while releasing plugin resources: {message}", ex.Message);
            }
        }

        public void Dispose()
        {
            List<Exception> exceptions = [];
            foreach (var plugin in _plugins.ToArray())
            {
                try
                {
                    Dispose(plugin);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Count > 0)
                throw new AggregateException("Failed to release plugins", exceptions);
        }

        public IReadOnlyList<PluginInfo> GetPlugins()
            => [.. _plugins];

        public IReadOnlyList<PluginInfo> GetLoadedPlugins()
            => [.. _plugins.Where(p => p.Lifetime == PluginLifetime.Loaded)];

        public IReadOnlyList<PluginInfo> GetInitializedPlugins()
            => [.. _plugins.Where(p => p.Lifetime == PluginLifetime.Initialized)];
    }
}
