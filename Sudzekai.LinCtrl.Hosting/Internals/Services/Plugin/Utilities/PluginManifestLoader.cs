using Microsoft.Extensions.Logging;
using Sudzekai.LinCtrl.Core;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Sudzekai.LinCtrl.Hosting
{
    internal class PluginManifestLoader(
        PluginManifestValidator validator,
        ILogger<PluginManifestLoader> logger)
    {
        public PluginManifest Load(string fullPath)
        {
            logger.LogInformation("Loading plugin manifest {fullPath}", fullPath);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"Plugin manifest file not found. (fullPath: {fullPath})");

            var json = File.ReadAllText(fullPath);

            logger.LogInformation("Manifest.json content: {content}", json);

            var pluginManifest = (PluginManifest)JsonSerializer.Deserialize(
                json,
                CoreContext.JsonSerializerOptions.GetTypeInfo(typeof(PluginManifest)))!;

            validator.ThrowIfInvalid(pluginManifest);

            var serialized = JsonSerializer.Serialize(
                pluginManifest,
                CoreContext.JsonSerializerOptions.GetTypeInfo(typeof(PluginManifest)));

            logger.LogInformation("Manifest after serialization: {serialized}", serialized);

            return pluginManifest;
        }

        public static bool TryFindManifest(string directoryPath, [NotNullWhen(true)] out string? fullPath)
        {
            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException($"Directory not found. (directoryPath: {directoryPath})");

            var files = Directory.EnumerateFiles(directoryPath);

            fullPath = files.FirstOrDefault(f => f.EndsWith("manifest.json"));

            if (!string.IsNullOrWhiteSpace(fullPath))
                return true;

            return false;
        }
    }
}
