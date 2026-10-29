using Sudzekai.WebOs.Core;
using System.Text.Json;
using System.Text.Json.Schema;
using System.Text.Json.Serialization.Metadata;

namespace Sudzekai.WebOs.Plugin
{

    internal sealed class OpenApiSchemaWriter : IOpenApiSchemaWriter
    {
        public void WriteContent(Utf8JsonWriter writer, ContentType contentType, JsonTypeInfo typeInfo)
        {
            writer.WriteStartObject(contentType.ToMediaTypeString());
            writer.WritePropertyName("schema");
            WriteSchema(writer, typeInfo);
            writer.WriteEndObject();
        }

        public void WriteEnvelopedContent(Utf8JsonWriter writer, ContentType contentType, int statusCode, JsonTypeInfo typeInfo)
        {
            writer.WriteStartObject(contentType.ToMediaTypeString());
            writer.WritePropertyName("schema");
            WriteEnvelopedSchema(writer, statusCode, typeInfo);
            writer.WriteEndObject();
        }

        private static void WriteSchema(Utf8JsonWriter writer, JsonTypeInfo typeInfo)
        {
            var schema = JsonSchemaExporter.GetJsonSchemaAsNode(typeInfo, new JsonSchemaExporterOptions
            {
                TreatNullObliviousAsNonNullable = true
            });

            if (schema is null)
            {
                writer.WriteStartObject();
                writer.WriteEndObject();
                return;
            }

            schema.WriteTo(writer);
        }

        private static void WriteEnvelopedSchema(Utf8JsonWriter writer, int statusCode, JsonTypeInfo typeInfo)
        {
            bool success = statusCode is > 199 and < 300;

            writer.WriteStartObject();
            writer.WriteString("type", "object");

            writer.WriteStartObject("properties");

            writer.WritePropertyName("success");
            writer.WriteStartObject();
            writer.WriteString("type", "boolean");
            writer.WriteEndObject();

            if (success)
            {
                writer.WritePropertyName("data");
                WriteSchema(writer, typeInfo);
            }
            else
            {
                writer.WritePropertyName("error");
                writer.WriteStartObject();
                writer.WriteString("type", "object");

                writer.WriteStartObject("properties");

                writer.WritePropertyName("message");
                writer.WriteStartObject();
                writer.WriteString("type", "string");
                writer.WriteEndObject();

                writer.WritePropertyName("code");
                writer.WriteStartObject();
                writer.WriteString("type", "integer");
                writer.WriteEndObject();

                writer.WriteEndObject();

                writer.WriteStartArray("required");
                writer.WriteStringValue("message");
                writer.WriteStringValue("code");
                writer.WriteEndArray();

                writer.WriteEndObject();
            }

            writer.WriteEndObject();

            writer.WriteStartArray("required");
            writer.WriteStringValue("success");
            writer.WriteStringValue(success ? "data" : "error");
            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}