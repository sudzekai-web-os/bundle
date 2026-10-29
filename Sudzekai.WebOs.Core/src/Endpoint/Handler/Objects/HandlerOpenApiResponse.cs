using System.Text.Json.Serialization.Metadata;

namespace Sudzekai.WebOs.Core
{
    public sealed class HandlerOpenApiResponse
    {
        public HandlerOpenApiResponse(
            int statusCode,
            JsonTypeInfo? typeInfo = null,
            string? description = null,
            ContentType contentType = ContentType.ApplicationJson)
        {
            StatusCode = statusCode;
            TypeInfo = typeInfo;
            Description = description;
            ContentType = contentType;
        }

        public int StatusCode { get; }

        public JsonTypeInfo? TypeInfo { get; }

        public string? Description { get; }

        public ContentType ContentType { get; }
    }
}
