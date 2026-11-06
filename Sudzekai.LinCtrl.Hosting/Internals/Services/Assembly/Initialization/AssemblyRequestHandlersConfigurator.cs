using Microsoft.Extensions.Logging;

namespace Sudzekai.LinCtrl.Hosting.Internals.Services.Assembly.Initialization
{
    internal class AssemblyRequestHandlersConfigurator(
        RequestHandlerLifetimeHost handlerHost,
        ILogger<AssemblyRequestHandlersConfigurator> logger) : IAssemblyInitializer
    {
        public void Initialize(AssemblyInfo info)
        {
            logger.LogDebug("[{tag}] Configuring assembly request handlers...", info.GetTag());

            int c = 0;
            foreach (var handler in info.Handlers)
            {
                try
                {
                    handlerHost.Build(handler);
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
