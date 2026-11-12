using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Core;
using System.Buffers;
using System.Text.Json;

namespace Sudzekai.LinCtrl.Hosting
{
    internal static class ResponseEnvelopeWriter
    {
        public static bool IsSuccess(int statusCode)
            => statusCode is >= 200 and < 300;

        public static bool AllowsBody(int statusCode)
            => statusCode >= 200 && statusCode is not (204 or 205 or 304);

        public static async Task WriteAsync(HttpContext context, HttpResponseDto response)
        {
            var buffer = new ArrayBufferWriter<byte>();
            using var writer = new Utf8JsonWriter(buffer);

            var statusCode = (int)response.StatusCode;
            try
            {
                WriteEnvelope(writer, response, statusCode);
                writer.Flush();
            }
            catch (JsonException)
            {
                statusCode = StatusCodes.Status500InternalServerError;
                buffer.Clear();
                writer.Reset(buffer);
                WriteEnvelope(writer, new HttpResponseDto(), statusCode);
                writer.Flush();
            }

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.ContentLength = buffer.WrittenCount;
            if (!HttpMethods.IsHead(context.Request.Method))
                await context.Response.Body.WriteAsync(buffer.WrittenMemory, context.RequestAborted);
        }

        private static void WriteEnvelope(Utf8JsonWriter writer, HttpResponseDto response, int statusCode)
        {
            var success = IsSuccess(statusCode);
            writer.WriteStartObject();
            writer.WriteBoolean("success", success);
            if (success)
            {
                writer.WritePropertyName("data");
                WriteData(writer, response);
            }
            else
            {
                writer.WriteStartObject("error");
                var (code, message) = ReadError(response, statusCode);
                writer.WriteNumber("code", code);
                writer.WriteString("message", message);
                writer.WriteEndObject();
            }
            writer.WriteEndObject();
        }

        private static (int Code, string Message) ReadError(HttpResponseDto response, int statusCode)
        {
            if (response.ContentType == ResponseContentType.ApplicationJson && response.Data.Length > 0)
            {
                try
                {
                    using var json = JsonDocument.Parse(response.Data);
                    var root = json.RootElement;
                    if (root.ValueKind == JsonValueKind.Object && root.EnumerateObject().Count() == 2
                        && root.TryGetProperty("code", out var code) && code.ValueKind == JsonValueKind.Number
                        && code.TryGetInt32(out var value) && root.TryGetProperty("message", out var message)
                        && message.ValueKind == JsonValueKind.String)
                        return (value, message.GetString()!);
                }
                catch (JsonException)
                {
                }
            }
            return (statusCode, ReasonPhrases.GetReasonPhrase(statusCode));
        }

        private static void WriteData(Utf8JsonWriter writer, HttpResponseDto response)
        {
            if (response.Data.Length == 0)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteRawValue(response.Data);
        }
    }
}
