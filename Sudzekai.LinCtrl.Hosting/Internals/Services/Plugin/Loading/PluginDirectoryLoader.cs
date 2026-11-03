using Microsoft.Extensions.Logging;

namespace Sudzekai.LinCtrl.Hosting
{
    internal class PluginDirectoryLoader(
        AssemblyLoader assemblyLoader,
        PluginManifestLoader manifestLoader,
        ILogger<PluginDirectoryLoader> logger)
    {
        public PluginInfo Load(string directoryPath)
        {
            logger.LogInformation("Loading plugin from {directoryPath}", directoryPath);

            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException($"Plugin directory not found. (directoryPath: {directoryPath})");

            if (!PluginManifestLoader.TryFindManifest(directoryPath, out var manifestPath))
                throw new InvalidOperationException($"Plugin directory doesn't contain manifest.json file. (directoryPath: {directoryPath})");

            var manifest = manifestLoader.Load(manifestPath);

            List<AssemblyInfo> assemblyInfos = [];

            foreach (var assemblyName in manifest.Assemblies)
            {
                var assemblyFullName = Path.Combine(directoryPath, assemblyName);

                assemblyInfos.Add(assemblyLoader.Load(assemblyFullName));
            }

            return new(manifest, assemblyInfos);
        }
    }
}
