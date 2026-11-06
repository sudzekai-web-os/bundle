using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Sudzekai.LinCtrl.Hosting
{
    internal class RequestHandlerDelegateBuilder(
        IEnumerable<IResponseWriter> responseWriters,
        ILogger<RequestHandlerDelegateBuilder> logger) : IRequestHandlerBuilder
    {
        public void Build(HttpRequestHandlerBuilder builder)
        {
            logger.LogDebug("[{tag}] Building delegate...", builder.GetTag());

            builder.ConventionBuilder.Add(c =>
            {
                c.RequestDelegate = GetRequestDelegate(builder);
            });

            logger.LogDebug("[{tag}] Delegate built", builder.GetTag());
        }

        private RequestDelegate GetRequestDelegate(HttpRequestHandlerBuilder builder)
        {
            return async (ctx) =>
            {
                var request = await ctx.Request.ToRequestDto();

                var response = await builder.Handler.HandleAsync(request);

                ctx.Response.StatusCode = (int)response.StatusCode;

                if (response.StatusCode == HttpStatusCode.NoContent ||
                    response.StatusCode == HttpStatusCode.NotModified ||
                    (int)response.StatusCode is >= 100 and < 200)
                    return;

                var isHead = HttpMethods.IsHead(ctx.Request.Method);

                ctx.Response.ContentType = response.ContentType.ToMimeString() + "; charset=utf-8";

                if (isHead)
                {
                    if (response.Data is { Length: > 0 })
                        ctx.Response.ContentLength = response.Data.Length;
                    return;
                }

                if (response.Data is { Length: > 0 })
                {
                    var writer = responseWriters.FirstOrDefault(w => w.ContentType == response.ContentType);

                    if (writer == null)
                    {
                        ctx.Response.ContentLength = response.Data.Length;
                        await ctx.Response.Body.WriteAsync(response.Data, ctx.RequestAborted);
                        return;
                    }

                    await writer.WriteAsync(ctx.Response, response.Data);
                }
                else
                {
                    ctx.Response.ContentLength = 0;
                }
            };
        }
    }
}
