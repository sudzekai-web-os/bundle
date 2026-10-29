using System.Text.Json.Serialization;

namespace Sudzekai.WebOs.Hosting
{
    [JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
    [JsonSerializable(typeof(PluginManifest))]
    internal partial class HostingJsonContext : JsonSerializerContext;
}
