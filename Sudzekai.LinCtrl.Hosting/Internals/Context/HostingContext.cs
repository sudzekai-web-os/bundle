using Sudzekai.LinCtrl.Abi;
using Sudzekai.LinCtrl.Abi.Allocation;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Core;
using Sudzekai.LinCtrl.Hosting.Objects;
using System.Text.Json.Serialization;

namespace Sudzekai.LinCtrl.Hosting
{
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true)]
    [JsonSerializable(typeof(LoginCommand))]
    [JsonSerializable(typeof(LoginToken))]
    [JsonSerializable(typeof(PluginManifest))]
    internal partial class HostingContext : JsonSerializerContext
    {
    }
}
