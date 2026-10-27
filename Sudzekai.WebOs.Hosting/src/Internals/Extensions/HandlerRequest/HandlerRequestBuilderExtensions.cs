using Microsoft.AspNetCore.Http;
using Sudzekai.WebOs.Core;
using System.Globalization;

namespace Sudzekai.WebOs.Hosting
{
    internal static class HandlerRequestBuilderExtensions
    {
        extension(HandlerRequest)
        {
            public static async Task<HandlerRequest> FromHttpRequest(HttpRequest request)
            {
                using var ms = new MemoryStream();

                await request.Body.CopyToAsync(ms);

                var body = ms.ToArray();

                return new()
                {
                    HttpMethod = ToMethod(request.Method),
                    
                    Host = request.Host.Value,
                    Path = request.Path,
                    Scheme = request.Scheme,
                    
                    QueryString = request.QueryString.Value ?? "",
                    
                    RouteValues = request.RouteValues.ToDictionary(kv => kv.Key, kv => kv.Value?.ToString() ?? string.Empty),
                    Headers = request.Headers.ToDictionary(kv => kv.Key, kv => kv.Value.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s!).ToArray()),
                    Cookies = request.Cookies.ToDictionary(kv => kv.Key, kv => kv.Value),
                    Claims = request.HttpContext.User.Claims.GroupBy(c => c.Type).ToDictionary(g => g.Key, g => g.Select(c => c.Value).ToArray()),

                    ContentType = request.ContentType,
                    Body = body
                };
            }
        }

        private static HandlerHttpMethod ToMethod(string method)
            => method.ToLower() switch
            {
                "get" => HandlerHttpMethod.Get,
                "post" => HandlerHttpMethod.Post,
                "put" => HandlerHttpMethod.Put,
                "patch" => HandlerHttpMethod.Patch,
                "delete" => HandlerHttpMethod.Delete,
                _ => throw new NotSupportedException(LogsDictionary.HandlerRequestBuilderExtensions.InvalidHttpMethod)
            };

    }
}