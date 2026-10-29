using Sudzekai.WebOs.Core;
using System.Buffers;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Schema;
using System.Text.Json.Serialization.Metadata;
using System.Text.RegularExpressions;

namespace Sudzekai.WebOs.Plugin
{
    internal static partial class HandlerOpenApiMetadataWriter
    {
        public static string Write(IReadOnlyList<IEndpointHandler> handlers)
        {
            var buffer = new ArrayBufferWriter<byte>();

            using (var writer = new Utf8JsonWriter(buffer, new JsonWriterOptions
            {
                Indented = false,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }))
            {
                writer.WriteStartObject();

                writer.WriteString("openapi", "3.1.0");

                writer.WriteStartObject("info");
                writer.WriteString("title", "Plugin");
                writer.WriteString("version", "1.0.0");
                writer.WriteEndObject();

                WriteTags(writer, handlers);

                writer.WriteStartObject("paths");

                var groups = handlers
                    .Where(h => h.OpenApiInfo is not null)
                    .GroupBy(h => NormalizePath(h.Pattern));

                foreach (var group in groups)
                {
                    writer.WriteStartObject(group.Key);

                    foreach (var handler in group)
                    {
                        writer.WriteStartObject(handler.HttpMethod.ToMethodString());
                        WriteOperation(writer, handler);
                        writer.WriteEndObject();
                    }

                    writer.WriteEndObject();
                }

                writer.WriteEndObject();
                writer.WriteEndObject();
            }

            return Encoding.UTF8.GetString(buffer.WrittenSpan);
        }

        private static void WriteTags(Utf8JsonWriter writer, IReadOnlyList<IEndpointHandler> handlers)
        {
            var tags = handlers
                .Where(h => h.OpenApiInfo is not null)
                .SelectMany(h => h.OpenApiInfo!.Tags)
                .Distinct(StringComparer.Ordinal)
                .ToArray();

            if (tags.Length == 0)
                return;

            writer.WriteStartArray("tags");

            foreach (var tag in tags)
            {
                writer.WriteStartObject();
                writer.WriteString("name", tag);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        private static void WriteOperation(Utf8JsonWriter writer, IEndpointHandler handler)
        {
            var info = handler.OpenApiInfo!;

            if (!string.IsNullOrEmpty(info.Name))
                writer.WriteString("operationId", info.Name);

            if (!string.IsNullOrEmpty(info.Summary))
                writer.WriteString("summary", info.Summary);

            if (!string.IsNullOrEmpty(info.Description))
                writer.WriteString("description", info.Description);

            if (info.Tags.Length > 0)
            {
                writer.WriteStartArray("tags");
                foreach (var tag in info.Tags)
                    writer.WriteStringValue(tag);
                writer.WriteEndArray();
            }

            WritePathParameters(writer, handler.Pattern);

            if (info.RequestBody is { } requestBody)
            {
                writer.WriteStartObject("requestBody");

                writer.WriteBoolean("required", requestBody.Required);

                if (!string.IsNullOrEmpty(requestBody.Description))
                    writer.WriteString("description", requestBody.Description);

                writer.WriteStartObject("content");
                WriteContent(writer, requestBody.ContentType, requestBody.TypeInfo);
                writer.WriteEndObject();

                writer.WriteEndObject();
            }

            writer.WriteStartObject("responses");

            if (info.Responses.Length == 0)
                WriteResponse(writer, new HandlerOpenApiResponse(200));
            else
                foreach (var response in info.Responses)
                    WriteResponse(writer, response);

            writer.WriteEndObject();
        }

        private static void WriteResponse(Utf8JsonWriter writer, HandlerOpenApiResponse response)
        {
            writer.WriteStartObject(response.StatusCode.ToString(CultureInfo.InvariantCulture));

            writer.WriteString("description", response.Description ?? DefaultDescription(response.StatusCode));

            if (response.TypeInfo is not null)
            {
                writer.WriteStartObject("content");
                WriteContent(writer, response.ContentType, response.TypeInfo);
                writer.WriteEndObject();
            }

            writer.WriteEndObject();
        }

        private static void WriteContent(Utf8JsonWriter writer, ContentType contentType, JsonTypeInfo typeInfo)
        {
            writer.WriteStartObject(contentType.ToMediaTypeString());
            writer.WritePropertyName("schema");
            WriteSchema(writer, typeInfo);
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

        private static void WritePathParameters(Utf8JsonWriter writer, string pattern)
        {
            var matches = PathParameterRegex().Matches(pattern);

            if (matches.Count == 0)
                return;

            writer.WriteStartArray("parameters");

            foreach (Match match in matches)
            {
                writer.WriteStartObject();
                writer.WriteString("name", match.Groups[1].Value);
                writer.WriteString("in", "path");
                writer.WriteBoolean("required", true);
                writer.WriteStartObject("schema");
                writer.WriteString("type", "string");
                writer.WriteEndObject();
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        private static string DefaultDescription(int statusCode)
            => Enum.IsDefined(typeof(HttpStatusCode), statusCode)
                ? ((HttpStatusCode)statusCode).ToString()
                : "Response";

        private static string NormalizePath(string pattern)
            => "/" + PathParameterRegex().Replace(pattern.TrimStart('/'), "{$1}");

        [GeneratedRegex(@"\{([^}:]+)(?::[^}]*)?\}")]
        private static partial Regex PathParameterRegex();
    }
}
