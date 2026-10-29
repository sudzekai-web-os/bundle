namespace Sudzekai.WebOs.Hosting
{
    internal class AssembliesRegistry
    {
        private readonly List<PluginAssembly> _plugins = [];

        public void AddPlugin(PluginAssembly plugin)
            => _plugins.Add(plugin);

        public IReadOnlyList<PluginAssembly> GetPluginsList()
            => _plugins;

        public void UnloadPlugins()
            => _plugins.ForEach(p => p.Dispose());
    }
}
