using Sudzekai.WebOs.Core;
using System.Buffers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Sudzekai.WebOs.Plugin
{

    internal sealed class OpenApiMetadataWriter : IOpenApiMetadataWriter
    {
        private readonly IOpenApiOperationWriter _operationWriter;

        public OpenApiMetadataWriter(IOpenApiOperationWriter operationWriter)
        {
            _operationWriter = operationWriter;
        }

        public string Write(IReadOnlyList<IEndpointHandler> handlers)
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
                    .GroupBy(h => OpenApiPathParameters.NormalizePath(h.Pattern));

                foreach (var group in groups)
                {
                    writer.WriteStartObject(group.Key);

                    foreach (var handler in group)
                    {
                        writer.WriteStartObject(handler.HttpMethod.ToMethodString());
                        _operationWriter.WriteOperation(writer, handler);
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
    }
}