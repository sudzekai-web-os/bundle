using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Sudzekai.WebOs.Hosting
{
    internal class PluginHost
    {
        private readonly PluginLoader _loader;
        private readonly PluginsRegistry _registry;
        private readonly ILogger<PluginHost> _logger;

        public PluginHost(PluginLoader loader, PluginsRegistry registry, ILogger<PluginHost> logger)
        {
            _loader = loader;
            _registry = registry;
            _logger = logger;
        }

        public async Task LoadPluginsAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Поиск плагинов...");

            var plugins = await LoadPlugins();

            foreach (var plugin in plugins)
                _registry.AddPlugin(plugin);
        }

        private async Task<List<Plugin>> LoadPlugins()
        {
            var pluginsDirPath = Path.Combine(AppContext.BaseDirectory, "plugins");

            if (!Directory.Exists(pluginsDirPath))
            {
                _logger.LogDebug("Создана директория plugins");
                Directory.CreateDirectory(pluginsDirPath);
                return [];
            }

            var dirPaths = Directory.GetDirectories(pluginsDirPath);

            if (dirPaths.Length == 0)
                return [];

            List<Plugin> result = [];

            foreach (var dirPath in dirPaths)
            {
                var pluginDirPath = Path.Combine(pluginsDirPath, dirPath);
                var pluginDirTag = Path.GetFileName(pluginDirPath);

                var jsonFiles = Directory.GetFiles(pluginDirPath).Where(f => f.EndsWith(".json"));

                _logger.LogDebug("[{pluginDirTag}] JSON файлы директории: {files}", pluginDirTag, string.Join(", ", jsonFiles.Select(Path.GetFileName)));

                var manifestFile = jsonFiles.FirstOrDefault(f => Path.GetFileName(f) == "manifest.json");

                _logger.LogDebug("[{pluginDirTag}] Найден манифест: {manifestFileName}", pluginDirTag, Path.GetFileName(manifestFile));
                _logger.LogInformation("[{pluginDirTag}] Директория сборок плагинов инициализирована", pluginDirTag);

                if (manifestFile == null)
                    continue;

                var manifestJson = await File.ReadAllTextAsync(manifestFile);

                var manifest = JsonSerializer.Deserialize(manifestJson, HostJsonContext.Default.PluginManifest);

                if (manifest is null)
                {
                    _logger.LogWarning("manifest.json директории {pluginDirTag} содержит невалидный json. Загрузка плагинов из неё пропускается...", pluginDirPath);
                    continue;
                }

                _logger.LogDebug("[{pluginDirTag}] Манифест загружен. Сборки манифеста: {assemblies}", pluginDirTag, string.Join(", ", manifest.Assemblies));

                var count = 0;
                foreach (var assembly in manifest.Assemblies)
                {
                    Plugin plugin;

                    try
                    {
                        plugin = _loader.Load(Path.Combine(pluginDirPath, assembly));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("[{pluginDirTag}|{assemblyFileName}] {ex}. Загрузка плагина пропускается", pluginDirTag, Path.GetFileName(assembly), ex.Message);
                        continue;
                    }

                    result.Add(plugin);
                    count++;
                }

                _logger.LogInformation("[{pluginDirTag}] Загружено {count} сборок", pluginDirTag, count);
            }

            return result;
        }
    }
}
