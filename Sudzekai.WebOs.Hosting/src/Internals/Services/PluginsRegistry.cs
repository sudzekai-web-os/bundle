namespace Sudzekai.WebOs.Hosting
{
    internal class PluginsRegistry
    {
        private readonly List<Plugin> _plugins = [];

        public void AddPlugin(Plugin plugin)
            => _plugins.Add(plugin);

        public IReadOnlyList<Plugin> GetPluginsList()
            => _plugins;

        public void UnloadPlugins()
            => _plugins.ForEach(p => p.Dispose());
    }
}
