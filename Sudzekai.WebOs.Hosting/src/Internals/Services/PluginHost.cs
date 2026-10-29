using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Sudzekai.WebOs.Hosting
{
    internal class PluginHost
    {
        private readonly AssemblyLoader _loader;
        private readonly AssembliesRegistry _registry;
        private readonly ILogger<PluginHost> _logger;

        public PluginHost(AssemblyLoader loader, AssembliesRegistry registry, ILogger<PluginHost> logger)
        {
            _loader = loader;
            _registry = registry;
            _logger = logger;
        }

        public async Task LoadPluginsAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(LogsDictionary.PluginHost.PluginsSearchStarted);

            await LoadPluginsAsync();

            _logger.LogInformation(LogsDictionary.PluginHost.PluginsLoaded, _registry.GetPluginsList().Count);
        }

        private async Task LoadPluginsAsync()
        {
            var pluginsDirPath = Path.Combine(AppContext.BaseDirectory, "plugins");

            if (!Directory.Exists(pluginsDirPath))
            {
                _logger.LogDebug(LogsDictionary.PluginHost.PluginsDirCreated);
                Directory.CreateDirectory(pluginsDirPath);
                return;
            }

            var pluginDirs = Directory.GetDirectories(pluginsDirPath);

            if (pluginDirs.Length == 0)
                return;

            foreach (var dirPath in pluginDirs)
                await LoadAssemblies(pluginsDirPath, dirPath);
        }

        private async Task LoadAssemblies(string pluginsDirPath, string dirPath)
        {
            var assembliesDirPath = Path.Combine(pluginsDirPath, dirPath);
            var assembliesDirTag = Path.GetFileName(assembliesDirPath);

            var manifest = await GetPluginManifestAsync(assembliesDirPath);

            if (manifest is null)
            {
                _logger.LogWarning(LogsDictionary.PluginHost.ManifestIsInvalid, assembliesDirPath);
                return;
            }

            _logger.LogDebug(LogsDictionary.PluginHost.ManifestLoaded, assembliesDirTag, string.Join(", ", manifest.Assemblies));

            var count = 0;

            foreach (var assembly in manifest.Assemblies)
            {
                _logger.LogInformation(LogsDictionary.PluginHost.AssembliesLoadingStarted, assembliesDirTag);

				try
                {
                    LoadAssembly(Path.Combine(assembliesDirPath, assembly));
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(LogsDictionary.PluginHost.AssemblyException, assembliesDirTag, Path.GetFileName(assembly), ex.Message);
                }

                count++;
            }

            _logger.LogInformation(LogsDictionary.PluginHost.AssembliesLoaded, assembliesDirTag, count);
            return;
        }

        private void LoadAssembly(string assemblyPath)
        {
			var plugin = _loader.Load(assemblyPath);
			_registry.AddPlugin(plugin);
		}

        private static async Task<PluginManifest?> GetPluginManifestAsync(string directoryPath)
        {
			var jsonFiles = Directory.GetFiles(directoryPath).Where(f => f.EndsWith(".json"));

            var manifestFilePath = jsonFiles.FirstOrDefault(f => Path.GetFileName(f) == "manifest.json");
            
            if (string.IsNullOrWhiteSpace(manifestFilePath)) 
                return null;

            var json = await File.ReadAllTextAsync(manifestFilePath);

            return JsonSerializer.Deserialize(json, HostingJsonContext.Default.PluginManifest);
		}
    }
}
