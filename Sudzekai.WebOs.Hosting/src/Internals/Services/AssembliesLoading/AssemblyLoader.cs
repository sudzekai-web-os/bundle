using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace Sudzekai.WebOs.Hosting
{
    internal class AssemblyLoader
    {
        private readonly ILogger<AssemblyLoader> _logger;
        private readonly IEnumerable<IAssemblyLoadingStepHandler> _handlers;
        private readonly int _stepsCount;

        public AssemblyLoader(IServiceProvider provider)
        {
            _logger = provider.GetRequiredService<ILogger<AssemblyLoader>>();

            _handlers = provider.GetServices<IAssemblyLoadingStepHandler>();

            _stepsCount = _handlers.Count();
        }

        public PluginAssembly Load(string assemblyPath)
        {
            var assemblyTag = Path.GetFileName(Path.GetDirectoryName(assemblyPath)) + "|" + Path.GetFileName(assemblyPath);

            var lib = NativeLibrary.Load(assemblyPath);

            PluginAssembly plugin = new()
            {
                AssemblyPath = assemblyPath
            };

            _logger.LogInformation(LogsDictionary.AssemblyLoader.LoadingStarted, assemblyTag);

            var step = 0;

            foreach (var handler in _handlers)
            {
                step++;
                var result = handler.Handle(lib, plugin);

                _logger.LogDebug(result, assemblyTag, $"{step}/{_stepsCount}");
            }

            _logger.LogInformation(LogsDictionary.AssemblyLoader.AssemblyLoaded, assemblyTag);

            return plugin;
        }
    }
}
