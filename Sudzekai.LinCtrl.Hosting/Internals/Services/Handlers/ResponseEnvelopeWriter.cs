using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Core;
using System.Buffers;
using System.Text;
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
                writer.WriteNumber("code", response.Error?.Code ?? statusCode);
                writer.WriteString("message", response.Error?.Message ?? ReasonPhrases.GetReasonPhrase(statusCode));
                writer.WriteEndObject();
            }
            writer.WriteEndObject();
        }

        private static void WriteData(Utf8JsonWriter writer, HttpResponseDto response)
        {
            if (response.Data.Length == 0)
            {
                writer.WriteNullValue();
                return;
            }

            switch (response.ContentType)
            {
                case ResponseContentType.ApplicationJson:
                    writer.WriteRawValue(response.Data);
                    break;
                case ResponseContentType.TextPlain:
                case ResponseContentType.TextHtml:
                case ResponseContentType.TextCsv:
                case ResponseContentType.ApplicationFormUrlEncoded:
                case ResponseContentType.ImageSvgXml:
                    writer.WriteStringValue(Encoding.UTF8.GetString(response.Data));
                    break;
                default:
                    writer.WriteBase64StringValue(response.Data);
                    break;
            }
        }
    }
}
