using Sudzekai.LinCtrl.Core;
using Sudzekai.LinCtrl.Abi.Managed;
using System.Buffers;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Schema;
using System.Text.Json.Serialization.Metadata;
using System.Text.RegularExpressions;

namespace Sudzekai.LinCtrl.Core
{
    public static class OpenApiMetadataWriter
    {
        private static readonly Regex _pathParameters = new(@"\{\*{0,2}([^}:?=]+)(?::([^}?=]+))?[^}]*\}");
        private static readonly JsonSchemaExporterOptions _schemaOptions = new() { TreatNullObliviousAsNonNullable = true };

        public static string Write(
            IEnumerable<(string Pattern, HttpRequestMethod HttpMethod, HandlerMetadata? Metadata, bool IsFiltered)> handlers,
            Func<ResponseContentType, string> toMimeString)
        {
            var buffer = new ArrayBufferWriter<byte>();
            using var writer = new Utf8JsonWriter(buffer);
            writer.WriteStartObject();
            writer.WriteString("openapi", "3.1.0");
            writer.WriteStartObject("info");
            writer.WriteString("title", "Plugin");
            writer.WriteString("version", "1.0.0");
            writer.WriteEndObject();
            writer.WriteStartObject("paths");
            foreach (var group in handlers.Where(handler => handler.Metadata != null || handler.IsFiltered)
                .GroupBy(handler => NormalizePath(handler.Pattern)))
            {
                writer.WriteStartObject(group.Key);
                foreach (var handler in group)
                {
                    writer.WritePropertyName(handler.HttpMethod.ToString().ToLowerInvariant());
                    WriteOperation(writer, handler.Pattern, handler.Metadata, handler.IsFiltered, toMimeString);
                }
                writer.WriteEndObject();
            }
            writer.WriteEndObject();
            writer.WriteEndObject();
            writer.Flush();
            return Encoding.UTF8.GetString(buffer.WrittenSpan);
        }

        public static string NormalizePath(string pattern)
            => "/" + _pathParameters.Replace(pattern.TrimStart('/'), "{$1}");

        private static void WriteOperation(Utf8JsonWriter writer, string pattern, HandlerMetadata? info, bool isFiltered,
            Func<ResponseContentType, string> toMimeString)
        {
            writer.WriteStartObject();
            if (info != null)
            {
                WriteOptional(writer, "operationId", info.Name);
                WriteOptional(writer, "summary", info.Summary);
                WriteOptional(writer, "description", info.Description);
                if (info.GroupName != "v1")
                    writer.WriteString("x-linctrl-document", info.GroupName);
                if (info.Tags.Length > 0)
                {
                    writer.WriteStartArray("tags");
                    foreach (var tag in info.Tags)
                        writer.WriteStringValue(tag);
                    writer.WriteEndArray();
                }
                if (info.RequestBody is { } body)
                {
                    writer.WriteStartObject("requestBody");
                    writer.WriteBoolean("required", body.Required);
                    WriteOptional(writer, "description", body.Description);
                    WriteContent(writer, toMimeString(body.ContentType), () => WriteSchema(writer, body.TypeInfo, null));
                    writer.WriteEndObject();
                }
            }

            WriteParameters(writer, pattern, info?.Parameters ?? []);
            writer.WriteStartObject("responses");
            var responses = (info?.Responses ?? []).ToDictionary(response => response.StatusCode);
            if (!responses.Keys.Any(IsSuccess))
                responses.TryAdd(200, new(200));
            if (isFiltered)
            {
                if (!responses.Keys.Any(code => code is >= 400 and < 500))
                    responses.TryAdd(400, new(400));
                responses.TryAdd(500, new(500));
            }
            foreach (var response in responses.Values.OrderBy(response => response.StatusCode))
                WriteResponse(writer, response, isFiltered, toMimeString);
            writer.WriteEndObject();
            writer.WriteEndObject();
        }

        private static void WriteParameters(Utf8JsonWriter writer, string pattern, MetadataParameter[] parameters)
        {
            var pathParameters = _pathParameters.Matches(pattern);
            if (parameters.Length == 0 && pathParameters.Count == 0)
                return;

            writer.WriteStartArray("parameters");
            foreach (var match in pathParameters.Cast<Match>())
            {
                var name = match.Groups[1].Value;
                if (parameters.Any(parameter => parameter.Name == name && parameter.Location == "path"))
                    continue;
                WriteParameter(writer, name, "path", null, true, () =>
                    WriteType(writer, match.Groups[2].Value is "int" or "long" ? "integer" : "string"));
            }
            foreach (var parameter in parameters)
                WriteParameter(writer, parameter.Name, parameter.Location, parameter.Description,
                    parameter.Required || parameter.Location == "path", () => WriteSchema(writer, parameter.TypeInfo, null));
            writer.WriteEndArray();
        }

        private static void WriteParameter(Utf8JsonWriter writer, string name, string location, string? description,
            bool required, Action schema)
        {
            writer.WriteStartObject();
            writer.WriteString("name", name);
            writer.WriteString("in", location);
            writer.WriteBoolean("required", required);
            WriteOptional(writer, "description", description);
            writer.WritePropertyName("schema");
            schema();
            writer.WriteEndObject();
        }

        private static void WriteResponse(Utf8JsonWriter writer, MetadataResponse response, bool isFiltered,
            Func<ResponseContentType, string> toMimeString)
        {
            var code = response.StatusCode;
            writer.WriteStartObject(code.ToString(CultureInfo.InvariantCulture));
            writer.WriteString("description", response.Description ?? ((HttpStatusCode)code).ToString());
            if (code >= 200 && code is not (204 or 205 or 304))
            {
                if (isFiltered && response.ContentType == ResponseContentType.ApplicationJson)
                    WriteContent(writer, toMimeString(response.ContentType), () => WriteEnvelope(writer, response));
                else if (response.ContentType != ResponseContentType.None && (response.TypeInfo != null || response.Schema != null))
                    WriteContent(writer, toMimeString(response.ContentType), () => WriteSchema(writer, response.TypeInfo, response.Schema));
            }
            writer.WriteEndObject();
        }

        private static void WriteEnvelope(Utf8JsonWriter writer, MetadataResponse response)
        {
            var success = IsSuccess(response.StatusCode);
            writer.WriteStartObject();
            writer.WriteString("type", "object");
            writer.WriteStartObject("properties");
            writer.WriteStartObject("success");
            writer.WriteString("type", "boolean");
            writer.WriteBoolean("const", success);
            writer.WriteBoolean("default", success);
            writer.WriteEndObject();
            if (success)
            {
                writer.WritePropertyName("data");
                WriteSchema(writer, response.TypeInfo, response.Schema);
            }
            else
            {
                writer.WriteStartObject("error");
                writer.WriteString("type", "object");
                writer.WriteStartObject("properties");
                writer.WritePropertyName("code");
                WriteType(writer, "integer");
                writer.WritePropertyName("message");
                WriteType(writer, "string");
                writer.WriteEndObject();
                WriteRequired(writer, "code", "message");
                writer.WriteEndObject();
            }
            writer.WriteEndObject();
            WriteRequired(writer, "success", success ? "data" : "error");
            writer.WriteEndObject();
        }

        private static void WriteContent(Utf8JsonWriter writer, string mimeType, Action schema)
        {
            writer.WriteStartObject("content");
            writer.WriteStartObject(mimeType);
            writer.WritePropertyName("schema");
            schema();
            writer.WriteEndObject();
            writer.WriteEndObject();
        }

        private static void WriteSchema(Utf8JsonWriter writer, JsonTypeInfo? typeInfo, string? schema)
        {
            if (typeInfo != null)
                typeInfo.GetJsonSchemaAsNode(_schemaOptions).WriteTo(writer);
            else if (schema != null)
                writer.WriteRawValue(schema);
            else
                writer.WriteRawValue("{}");
        }

        private static void WriteRequired(Utf8JsonWriter writer, string first, string second)
        {
            writer.WriteStartArray("required");
            writer.WriteStringValue(first);
            writer.WriteStringValue(second);
            writer.WriteEndArray();
        }

        private static void WriteType(Utf8JsonWriter writer, string type)
        {
            writer.WriteStartObject();
            writer.WriteString("type", type);
            writer.WriteEndObject();
        }

        private static void WriteOptional(Utf8JsonWriter writer, string name, string? value)
        {
            if (!string.IsNullOrEmpty(value))
                writer.WriteString(name, value);
        }

        private static bool IsSuccess(int code)
            => code is >= 200 and < 300;
    }
}
