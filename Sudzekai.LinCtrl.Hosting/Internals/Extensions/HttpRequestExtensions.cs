using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Hosting
{
    internal static class HttpRequestExtensions
    {
        extension(Microsoft.AspNetCore.Http.HttpRequest req)
        {
            public async Task<HttpRequestDto> ToRequestDto()
            {
                byte[] body;

                if (req.ContentLength is > 0 and var length)
                {
                    body = new byte[length];
                    await req.Body.ReadExactlyAsync(body);
                }
                else
                {
                    using var ms = new MemoryStream();
                    await req.Body.CopyToAsync(ms);
                    body = ms.ToArray();
                }

                return new()
                {
                    HttpMethod = RequestHandlerHttpMethod.FromHttpMethodString(req.Method),
                    Host = req.Host.Host,
                    Scheme = req.Scheme,
                    Path = req.Path.ToString(),
                    QueryString = req.QueryString.ToString(),

                    Headers = req.Headers.ToDictionary(p => p.Key, p => p.Value.OfType<string>().ToArray()),
                    Claims = req.HttpContext.User.Claims
                        .GroupBy(c => c.Type)
                        .ToDictionary(g => g.Key, g => g.Select(c => c.Value).ToArray()),

                    RouteValues = req.RouteValues.ToDictionary(p => p.Key, p => p.Value?.ToString() ?? ""),
                    Cookies = req.Cookies.ToDictionary(),

                    ContentType = ContentType.FromMimeString(req.ContentType),

                    Body = body
                };
            }
        }
    }
}
