using LinCtrlAssembly.Objects;
using System.Text.Json.Serialization;

namespace LinCtrlAssembly.Context
{
    [JsonSerializable(typeof(ResponseObject))]
    internal partial class AssemblyJsonContext : JsonSerializerContext;
}
