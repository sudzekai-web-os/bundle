using Sudzekai.LinCtrl.Core;
using System.Text.Json;
using AspHttpResponse = Microsoft.AspNetCore.Http.HttpResponse;

namespace Sudzekai.LinCtrl.Hosting.Internals.Services.Handlers.Services
{
    internal class JsonResponseWriter : IResponseWriter
    {
        public ResponseContentType ContentType => ResponseContentType.ApplicationJson;

        public async Task WriteAsync(AspHttpResponse response, byte[] data)
        {
            response.ContentLength = data.Length;

            using var writer = new Utf8JsonWriter(response.Body);

            writer.WriteRawValue(data);

            await writer.FlushAsync();
        }
    }
}
