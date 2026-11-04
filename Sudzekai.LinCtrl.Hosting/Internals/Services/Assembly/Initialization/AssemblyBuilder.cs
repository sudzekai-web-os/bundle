using Microsoft.Extensions.Logging;

namespace Sudzekai.LinCtrl.Hosting
{
    internal class AssemblyBuilder(
        ILogger<AssemblyBuilder> logger) : IAssemblyInitializer
    {
        public void Initialize(AssemblyInfo info)
        {
            logger.LogDebug("[{tag}] Building assembly...", info.GetTag());

            if (info.VTable.BuildDelegate == null)
                throw new InvalidOperationException("Assembly delegate for building was null");

            info.VTable.BuildDelegate();
         
            logger.LogDebug("[{tag}] Assembly built", info.GetTag());
        }
    }
}
