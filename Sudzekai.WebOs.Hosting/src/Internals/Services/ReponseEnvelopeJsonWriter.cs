using Microsoft.AspNetCore.Http;
using Sudzekai.WebOs.Core;
using System.Text.Json;

namespace Sudzekai.WebOs.Hosting
{
    internal static class ReponseEnvelopeJsonWriter
    {
        public static async Task WriteAsync(HttpContext ctx, HandlerResult result)
        {
            bool success = result.StatusCode is > 199 and < 300;
            var dataStr = result.Data?.ToString();
            var isJson = IsValidJsonObject(dataStr ?? "");

            using Utf8JsonWriter jsonWriter = new(ctx.Response.BodyWriter);

            jsonWriter.WriteStartObject();

            if (success)
            {
                WriteSuccessBody(dataStr, isJson, jsonWriter);
            }
            else
            {
                WriteErroredBody(result, dataStr, isJson, jsonWriter);
            }

            jsonWriter.WriteEndObject();

            await jsonWriter.FlushAsync();
        }

        private static void WriteErroredBody(
            HandlerResult result,
            string? dataStr,
            bool isJson,
            Utf8JsonWriter jsonWriter)
        {
            jsonWriter.WriteBoolean("success", false);

            jsonWriter.WriteStartObject("error");

            if (!string.IsNullOrWhiteSpace(dataStr) && isJson)
            {
                jsonWriter.WritePropertyName("message");
                jsonWriter.WriteRawValue(dataStr);
            }
            else
                jsonWriter.WriteNull("message");

            jsonWriter.WriteNumber("code", result.StatusCode);

            jsonWriter.WriteEndObject();
        }

        private static void WriteSuccessBody(
            string? dataStr,
            bool isJson,
            Utf8JsonWriter jsonWriter)
        {
            jsonWriter.WriteBoolean("success", true);

            if (!string.IsNullOrWhiteSpace(dataStr) && isJson)
            {
                jsonWriter.WritePropertyName("data");
                jsonWriter.WriteRawValue(dataStr);
            }
            else
                jsonWriter.WriteNull("data");
        }

        private static bool IsValidJsonObject(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return false;

            try
            {
                using var doc = JsonDocument.Parse(json);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }
    }
}
