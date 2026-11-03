using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Hosting 
{
    internal class RequestHandlerUnloader(
        IEnumerable<IRequestHandlerUnloadingStepHandler> stepHandlers,
        ILogger<RequestHandlerUnloader> logger)
    {
        public IEndpointRouteBuilder RouteBuilder
        {
            get => field ?? throw new InvalidOperationException("RouteBuilder was null");
            set;
        }

        public void Unload(IHttpRequestHandler handler)
        {
            logger.LogInformation("Unloading request handler {pattern}", handler.Pattern);

            RouteBuilder.Map(handler.Pattern, () => Results.NotFound());

            foreach (var stepHandler in stepHandlers)
                stepHandler.Unload(handler);
        }
    }
}
