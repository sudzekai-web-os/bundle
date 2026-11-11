using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Sudzekai.LinCtrl.Abi.Managed;
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
                if (builder.Handler.IsFiltering)
                    c.Metadata.Add(c.RequestDelegate.Method);
            });

            if (builder.Handler.IsFiltering)
                builder.ConventionBuilder.Finally(ResponseEnvelopeMetadata.Apply);

            logger.LogDebug("[{tag}] Delegate built", builder.GetTag());
        }

        private RequestDelegate GetRequestDelegate(HttpRequestHandlerBuilder builder)
        {
            return async (ctx) =>
            {
                var request = await ctx.Request.ToRequestDto();

                HttpResponseDto response;
                try
                {
                    response = builder.Handler.Handle(request);
                }
                catch (Exception ex) when (builder.Handler.IsFiltering && ex is not OperationCanceledException)
                {
                    logger.LogError(ex, "[{tag}] Request handler failed", builder.GetTag());
                    response = new HttpResponseDto { StatusCode = HttpStatusCode.InternalServerError };
                }

                ctx.Response.StatusCode = (int)response.StatusCode;

                if (builder.Handler.IsFiltering)
                {
                    if (ResponseEnvelopeWriter.AllowsBody((int)response.StatusCode))
                        await ResponseEnvelopeWriter.WriteAsync(ctx, response);
                    return;
                }

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
