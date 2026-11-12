using Sudzekai.LinCtrl.Core;
using System.Text.Json.Serialization.Metadata;

namespace Sudzekai.LinCtrl.Abi.Managed
{
    public sealed class MetadataRequestBody
    {
        public MetadataRequestBody(
            JsonTypeInfo typeInfo,
            string? description = null,
            bool required = true,
            ResponseContentType contentType = ResponseContentType.ApplicationJson)
        {
            TypeInfo = typeInfo;
            Description = description;
            Required = required;
            ContentType = contentType;
        }

        public JsonTypeInfo TypeInfo { get; }

        public string? Description { get; }

        public bool Required { get; }

        public ResponseContentType ContentType { get; }
    }
}
