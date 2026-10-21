using Sudzekai.WebOs.Core;

namespace Sudzekai.WebOs.Hosting
{
    public class Plugin
    {
        public string AssemblyPath { get; internal set; } = string.Empty;

        public PluginInfo Info { get; internal set; } = new();

        public EndpointHandler[] Handlers { get; internal set; } = [];

        internal Action? DisposeFunc { get; set; }

        internal void Dispose()
            => DisposeFunc?.Invoke();
    }
}
