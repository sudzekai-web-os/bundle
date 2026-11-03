using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Hosting
{
    internal class RequestHandlerLoader(
        IEnumerable<IEndpointConventionConfiguringStepHandler> stepHandlers,
        ILogger<RequestHandlerLoader> logger)
    {
        public IEndpointRouteBuilder RouteBuilder
        {
            get => field ?? throw new InvalidOperationException("RouteBuilder was null");
            set;
        }

        public void Load(IHttpRequestHandler handler)
        {
            logger.LogInformation("Loading request handler {pattern} ({method})", handler.Pattern, handler.HttpMethod);

            var builder = RouteBuilder.MapMethods(
                handler.Pattern,
                [handler.HttpMethod.ToMethodString()],
                GetRequestDelegate(handler.HandleAsync));

            foreach (var stepHandler in stepHandlers)
                stepHandler.Handle(builder);
        }

        private static RequestDelegate GetRequestDelegate(RequestHandleDelegate del)
            => async (HttpContext ctx) =>
            {
                var request = await ctx.Request.ToRequestDto();
                var response = await del(request);

                ctx.Response.ContentType = response.ContentType.ToMimeString() + "; charset=utf-8";
                ctx.Response.ContentLength = response.Data.Length;

                await ctx.Response.Body.WriteAsync(response.Data, ctx.RequestAborted);
            };
    }
}
