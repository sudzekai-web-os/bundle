using Sudzekai.WebOs.Core;
using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Sudzekai.WebOs.Plugin
{

    internal sealed class OpenApiOperationWriter : IOpenApiOperationWriter
    {
        private readonly IOpenApiSchemaWriter _schemaWriter;

        public OpenApiOperationWriter(IOpenApiSchemaWriter schemaWriter)
        {
            _schemaWriter = schemaWriter;
        }

        public void WriteOperation(Utf8JsonWriter writer, IEndpointHandler handler)
        {
            var info = handler.OpenApiInfo!;

            bool isFiltered = handler.FilteringInfo is not null;

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
                _schemaWriter.WriteContent(writer, requestBody.ContentType, requestBody.TypeInfo);
                writer.WriteEndObject();

                writer.WriteEndObject();
            }

            writer.WriteStartObject("responses");

            if (info.Responses.Length == 0)
                WriteResponse(writer, new HandlerOpenApiResponse(200), isFiltered);
            else
                foreach (var response in info.Responses)
                    WriteResponse(writer, response, isFiltered);

            writer.WriteEndObject();
        }

        private void WriteResponse(Utf8JsonWriter writer, HandlerOpenApiResponse response, bool isFiltered)
        {
            writer.WriteStartObject(response.StatusCode.ToString(CultureInfo.InvariantCulture));

            writer.WriteString("description", response.Description ?? DefaultDescription(response.StatusCode));

            if (response.TypeInfo is not null)
            {
                writer.WriteStartObject("content");

                if (isFiltered)
                    _schemaWriter.WriteEnvelopedContent(writer, response.ContentType, response.StatusCode, response.TypeInfo);
                else
                    _schemaWriter.WriteContent(writer, response.ContentType, response.TypeInfo);

                writer.WriteEndObject();
            }

            writer.WriteEndObject();
        }

        private static void WritePathParameters(Utf8JsonWriter writer, string pattern)
        {
            var matches = OpenApiPathParameters.Find(pattern);

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
    }
}