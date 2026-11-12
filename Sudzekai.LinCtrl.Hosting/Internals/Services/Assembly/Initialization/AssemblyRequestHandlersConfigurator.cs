using Microsoft.Extensions.Logging;
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
            using var document = string.IsNullOrWhiteSpace(payload) ? null : JsonDocument.Parse(payload);
            var paths = document?.RootElement.GetProperty("paths");

            int c = 0;
            foreach (var handler in info.Handlers)
            {
                try
                {
                    handlerHost.Build(handler, paths);
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
