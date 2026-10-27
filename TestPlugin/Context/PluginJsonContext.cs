using System.Text.Json.Serialization;
using TestPlugin.Object;

namespace TestPlugin.Context
{
    [JsonSerializable(typeof(TestObject))]
    [JsonSerializable(typeof(string))]
    internal partial class PluginJsonContext : JsonSerializerContext;
}
