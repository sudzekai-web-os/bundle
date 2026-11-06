using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Hosting
{
    internal class AssemblyLoggingDelegator(
        ILogger<AssemblyLoggingDelegator> logger,
        IServiceProvider provider) : IAssemblyInitializer
    {
        public unsafe void Initialize(AssemblyInfo info)
        {
            logger.LogDebug("[{tag}] Delegating logging...", info.GetTag());
            
            if (info.VTable.DelegateLoggingDelegate == null)
                throw new InvalidOperationException("Assembly delegate for delegate logging is null");

            info.VTable.DelegateLoggingDelegate((entry) =>
            {
                var factory = provider.GetRequiredService<ILoggerFactory>();

                var assemblyLogger = factory.CreateLogger(entry.Category);

                assemblyLogger.Log((Microsoft.Extensions.Logging.LogLevel)entry.LogLevel, entry.Message);
            });

            logger.LogDebug("[{tag}] Logging delegated", info.GetTag());
        }
    }
}
