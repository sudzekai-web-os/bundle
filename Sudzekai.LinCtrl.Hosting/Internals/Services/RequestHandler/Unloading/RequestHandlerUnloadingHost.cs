using Microsoft.Extensions.Logging;
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Hosting 
{
    internal class RequestHandlerUnloadingHost(
        RequestHandlerUnloader unloader,
        ILogger<RequestHandlerUnloadingHost> logger)
    {
        public void Unload(IHttpRequestHandler[] handlers)
        {
            logger.LogInformation("Unloading {count} request handlers", handlers.Length);

            foreach (var handler in handlers)
                unloader.Unload(handler);
        }
    }
}
