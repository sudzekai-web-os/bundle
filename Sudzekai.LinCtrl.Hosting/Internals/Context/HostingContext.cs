using Sudzekai.LinCtrl.Hosting.Objects;
using System.Text.Json.Serialization;

namespace Sudzekai.LinCtrl.Hosting
{
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true)]
    [JsonSerializable(typeof(int))]
    [JsonSerializable(typeof(long))]
    [JsonSerializable(typeof(LoginRequest))]
    [JsonSerializable(typeof(LoginResponse))]
    [JsonSerializable(typeof(PluginManifest))]
    internal partial class HostingContext : JsonSerializerContext { }
}
