using Microsoft.Extensions.Logging;

namespace Sudzekai.LinCtrl.Hosting
{
    internal class AssemblyRequestHandlersLoader(
        ILogger<AssemblyRequestHandlersLoader> logger) : IAssemblyInitializer
    {
        public void Initialize(AssemblyInfo info)
        {
            logger.LogDebug("[{tag}] Receiving assembly request handlers...", info.GetTag());

            if (info.VTable.GetRequestHandlersDelegate == null)
                throw new InvalidOperationException("Assembly delegate for get http request handlers was null");

            info.Handlers = info.VTable.GetRequestHandlersDelegate();

            logger.LogDebug("[{tag}] Assembly request handlers received (count: {count})", info.GetTag(), info.Handlers.Count);
        }
    }
}
