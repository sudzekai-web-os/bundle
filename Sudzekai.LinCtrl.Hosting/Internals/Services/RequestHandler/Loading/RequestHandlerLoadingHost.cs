using Microsoft.Extensions.Logging;
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Hosting 
{
    internal class RequestHandlerLoadingHost(
        RequestHandlerLoader loader,
        ILogger<RequestHandlerLoadingHost> logger)
    {
        public void Load(IHttpRequestHandler[] handlers)
        {
            logger.LogInformation("Loading {count} request handlers", handlers.Length);

            foreach (var handler in handlers)
                loader.Load(handler);
        }
    }
}
