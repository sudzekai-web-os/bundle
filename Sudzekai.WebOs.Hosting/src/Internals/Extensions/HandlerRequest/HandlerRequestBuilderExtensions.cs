using Microsoft.AspNetCore.Http;
using Sudzekai.WebOs.Core;

namespace Sudzekai.WebOs.Hosting
{
    internal static class HandlerRequestBuilderExtensions
    {
        extension(HandlerRequest)
        {
            public static async Task<HandlerRequest> FromHttpRequest(HttpRequest request)
                => new()
                {
                    HttpMethod = ToMethod(request.Method),
                    Path = request.Path,
                    QueryString = request.QueryString.Value ?? "",
                    Headers = [.. request.Headers.Select(ToHeaderEntry)],
                    Body = await ReadBodyAsync(request.Body),
                };
        }

        private static HandlerHttpMethod ToMethod(string method)
            => method.ToLower() switch
            {
                "get" => HandlerHttpMethod.Get,
                "post" => HandlerHttpMethod.Post,
                "put" => HandlerHttpMethod.Put,
                "patch" => HandlerHttpMethod.Patch,
                "delete" => HandlerHttpMethod.Delete,
                _ => throw new NotSupportedException("Был получен неподдерживаемый http method")
            };

        private static HeaderEntry ToHeaderEntry(KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues> kv)
            => new()
            {
                Key = kv.Key,
                Values = [.. kv.Value.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s!)]
            };

        private static async Task<byte[]> ReadBodyAsync(Stream body)
        {
            if (body == null) return [];

            using var ms = new MemoryStream();
            await body.CopyToAsync(ms);
            return ms.ToArray();
        }
    }
}