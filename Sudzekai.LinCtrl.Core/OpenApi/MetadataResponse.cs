using Sudzekai.LinCtrl.Core;
using System.Text.Json.Serialization.Metadata;

namespace Sudzekai.LinCtrl.Abi.Managed
{
    public sealed class MetadataResponse
    {
        public MetadataResponse(
            int statusCode,
            JsonTypeInfo? typeInfo = null,
            string? description = null,
            ResponseContentType contentType = ResponseContentType.ApplicationJson)
        {
            StatusCode = statusCode;
            TypeInfo = typeInfo;
            Description = description;
            ContentType = contentType;
        }

        public int StatusCode { get; }

        public string? Schema { get; init; }

        public JsonTypeInfo? TypeInfo { get; }

        public string? Description { get; }

        public ResponseContentType ContentType { get; }
    }
}
