using Sudzekai.LinCtrl.Core;
using Sudzekai.LinCtrl.Extensions;
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
                builder.Configure?.Invoke(c);
            });

            logger.LogDebug("[{tag}] Delegate built", builder.GetTag());
        }

        private RequestDelegate GetRequestDelegate(HttpRequestHandlerBuilder builder)
        {
            return async (ctx) =>
            {
                byte[]? body = null;
                if (builder.MaxRequestBodyBytes is { } limit)
                {
                    ctx.Response.Headers.CacheControl = "no-store";
                    if (ctx.Request.ContentLength > limit)
                    {
                        await WritePayloadTooLargeAsync(ctx, builder.IsFiltered);
                        return;
                    }

                    try
                    {
                        var buffer = new byte[limit + 1];
                        var length = await ctx.Request.Body.ReadAtLeastAsync(buffer, buffer.Length, false, ctx.RequestAborted);
                        if (length > limit)
                        {
                            await WritePayloadTooLargeAsync(ctx, builder.IsFiltered);
                            return;
                        }
                        body = buffer[..length];
                    }
                    catch (BadHttpRequestException ex) when (ex.StatusCode == StatusCodes.Status413PayloadTooLarge)
                    {
                        await WritePayloadTooLargeAsync(ctx, builder.IsFiltered);
                        return;
                    }
                }

                var request = await ctx.Request.ToRequestDto(body);

                HttpResponseDto response;
                try
                {
                    response = builder.Resolve(ctx.RequestServices).Handle(request);
                }
                catch (Exception ex) when (builder.IsFiltered && ex is not OperationCanceledException)
                {
                    logger.LogError(ex, "[{tag}] Request handler failed", builder.GetTag());
                    response = new HttpResponseDto { StatusCode = HttpStatusCode.InternalServerError, ContentType = ResponseContentType.ApplicationJson };
                }

                ctx.Response.StatusCode = (int)response.StatusCode;

                if (builder.IsFiltered && response.ContentType == ResponseContentType.ApplicationJson)
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

                if (response.ContentType != ResponseContentType.None)
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

        private static async Task WritePayloadTooLargeAsync(HttpContext context, bool isFiltered)
        {
            if (isFiltered)
                await ResponseEnvelopeWriter.WriteAsync(context, new HttpResponseDto { StatusCode = HttpStatusCode.RequestEntityTooLarge });
            else
                context.Response.StatusCode = StatusCodes.Status413PayloadTooLarge;
        }
    }
}
