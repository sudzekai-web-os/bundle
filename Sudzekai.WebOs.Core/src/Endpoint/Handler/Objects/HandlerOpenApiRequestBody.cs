using System.Text.Json.Serialization.Metadata;

namespace Sudzekai.WebOs.Core
{
    public sealed class HandlerOpenApiRequestBody
    {
        public HandlerOpenApiRequestBody(
            JsonTypeInfo typeInfo,
            string? description = null,
            bool required = true,
            ContentType contentType = ContentType.ApplicationJson)
        {
            TypeInfo = typeInfo;
            Description = description;
            Required = required;
            ContentType = contentType;
        }

        public JsonTypeInfo TypeInfo { get; }

        public string? Description { get; }

        public bool Required { get; }

        public ContentType ContentType { get; }
    }
}
