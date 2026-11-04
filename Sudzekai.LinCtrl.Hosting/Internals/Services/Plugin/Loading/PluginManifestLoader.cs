using Sudzekai.LinCtrl.Core;
using Sudzekai.LinCtrl.Hosting.Objects;
using System.Text.Json;

namespace Sudzekai.LinCtrl.Hosting
{
    internal class PluginManifestLoader : IPluginLoader
    {
        public void Load(PluginInfo info)
        {
            var manifestPath = Path.Combine(info.DirectoryPath, "manifest.json");

            if (!File.Exists(manifestPath))
                throw new FileNotFoundException($"Directory not contain manifest.json file. (path: {info.DirectoryPath}");

            var json = File.ReadAllText(manifestPath);

            info.Manifest = (PluginManifest)JsonSerializer.Deserialize(json, CoreContext.JsonSerializerOptions.GetTypeInfo(typeof(PluginManifest)))!;
        }
    }
}
