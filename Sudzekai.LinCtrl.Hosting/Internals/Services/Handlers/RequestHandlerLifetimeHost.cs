using Sudzekai.LinCtrl.Abi.Managed;
using Microsoft.Extensions.Logging;
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Hosting
{
    internal class RequestHandlerLifetimeHost(
        ILogger<RequestHandlerLifetimeHost> logger,
        IEnumerable<IRequestHandlerBuilder> builders,
        RequestHandlerEndpointDataSource dataSource)
    {
        public void Remove(IEnumerable<IRequestHandler> handlers)
            => dataSource.Remove(handlers);

        public HttpRequestHandlerBuilder Build(IRequestHandler requestHandler, RequestHandlerMetadata? metadata = null)
        {
            var builder = new HttpRequestHandlerBuilder(requestHandler) { Metadata = metadata };

            logger.LogInformation("[{tag}] Handler building started", builder.GetTag());

            try
            {
                foreach (var initializer in builders)
                    initializer.Build(builder);
            }
            catch
            {
                dataSource.Remove([requestHandler]);
                throw;
            }

            logger.LogInformation("[{tag}] Handler built", builder.GetTag());

            return builder;
        }
    }
}
