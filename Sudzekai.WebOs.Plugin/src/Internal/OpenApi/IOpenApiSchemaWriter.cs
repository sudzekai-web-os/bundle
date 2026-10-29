using Sudzekai.WebOs.Core;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Sudzekai.WebOs.Plugin
{
    internal interface IOpenApiSchemaWriter
    {
        void WriteContent(Utf8JsonWriter writer, ContentType contentType, JsonTypeInfo typeInfo);

        void WriteEnvelopedContent(Utf8JsonWriter writer, ContentType contentType, int statusCode, JsonTypeInfo typeInfo);
    }
}