using System.Text.Json.Serialization;

namespace Sudzekai.LinCtrl.Hosting
{

    [JsonSerializable(typeof(PluginManifest))]
    internal partial class HostingJsonContext : JsonSerializerContext;
}