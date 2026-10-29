using Sudzekai.WebOs.Core;

namespace Sudzekai.WebOs.Hosting
{
    public class PluginAssembly
    {
        public string AssemblyPath { get; internal set; } = string.Empty;

        public AssemblyInfo Info { get; internal set; } = new();

        internal IEndpointHandler[] EndpointHandlers { get; set; } = [];

        public IReadOnlyList<IEndpointHandler> Handlers => [.. EndpointHandlers];

        internal string? HandlersMetadataJson { get; set; }

        internal Action? DisposeAction { get; set; }

        internal void Dispose()
            => DisposeAction?.Invoke();
    }
}
