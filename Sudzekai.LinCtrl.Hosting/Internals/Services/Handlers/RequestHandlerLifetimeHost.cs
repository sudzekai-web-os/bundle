using Microsoft.Extensions.Logging;
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Hosting
{
    internal class RequestHandlerLifetimeHost(
        ILogger<RequestHandlerLifetimeHost> logger,
        IEnumerable<IRequestHandlerBuilder> builders)
    {
        public HttpRequestHandlerBuilder Build(IHttpRequestHandler requestHandler)
        {
            var builder = new HttpRequestHandlerBuilder(requestHandler);

            logger.LogInformation("[{tag}] Handler building started", builder.GetTag());

            foreach (var initializer in builders)
                initializer.Build(builder);

            logger.LogInformation("[{tag}] Handler built", builder.GetTag());

            return builder;
        }
    }
}
