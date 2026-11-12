using System.Text.Json.Serialization.Metadata;

namespace Sudzekai.LinCtrl.Abi.Managed
{
    public sealed class MetadataParameter(
        string name, string location, JsonTypeInfo typeInfo, string? description = null, bool required = false)
    {
        public string Name { get; } = name;
        public string Location { get; } = location;
        public JsonTypeInfo TypeInfo { get; } = typeInfo;
        public string? Description { get; } = description;
        public bool Required { get; } = required;
    }
}
