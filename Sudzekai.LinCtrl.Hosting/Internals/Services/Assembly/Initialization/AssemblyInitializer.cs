using Microsoft.Extensions.Logging;

namespace Sudzekai.LinCtrl.Hosting
{
    internal class AssemblyInitializer(
        ILogger<AssemblyInitializer> logger) : IAssemblyInitializer
    {
        public void Initialize(AssemblyInfo info)
        {
            logger.LogDebug("[{tag}] Initializing assembly...", info.GetTag());

            if (info.VTable.InitDelegate == null)
                throw new InvalidOperationException("Assembly delegate for initialization was null");

            info.VTable.InitDelegate();

            logger.LogDebug("[{tag}] Assembly initialized", info.GetTag());
        }
    }
}
