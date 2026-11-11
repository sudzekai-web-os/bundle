using Microsoft.Extensions.Logging;
using Sudzekai.LinCtrl.Abi.Managed;
using System.Text.Json;

namespace Sudzekai.LinCtrl.Hosting.Internals.Services.Assembly.Initialization
{
    internal class AssemblyRequestHandlersConfigurator(
        RequestHandlerLifetimeHost handlerHost,
        ILogger<AssemblyRequestHandlersConfigurator> logger) : IAssemblyInitializer
    {
        public void Initialize(AssemblyInfo info)
        {
            logger.LogDebug("[{tag}] Configuring assembly request handlers...", info.GetTag());

            var payload = info.VTable.GetRequestHandlersMetadataDelegate();
            var metadata = string.IsNullOrWhiteSpace(payload)
                ? []
                : JsonSerializer.Deserialize(payload, RequestHandlerMetadataContext.Default.RequestHandlerMetadataArray) ?? [];
            var metadataByRoute = metadata.ToDictionary(item => (item.HttpMethod, item.Pattern));

            int c = 0;
            foreach (var handler in info.Handlers)
            {
                try
                {
                    metadataByRoute.TryGetValue((handler.HttpMethod, handler.Pattern), out var description);
                    handlerHost.Build(handler, description);
                    c++;
                }
                catch (Exception ex)
                {
                    logger.LogError("[{tag}] Exception while configuring assembly request handler: {message}. Handler skipped", info.GetTag(), ex.Message);
                    continue;
                }
            }

            logger.LogDebug("[{tag}] Assembly request handlers configured (count: {count})", info.GetTag(), c);
        }
    }
}
