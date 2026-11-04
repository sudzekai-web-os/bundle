using Microsoft.Extensions.Logging;

namespace Sudzekai.LinCtrl.Hosting.Internals.Services.Assembly.Initialization
{
    internal class AssemblyInitializationFinalizer(
        ILogger<AssemblyInitializationFinalizer> logger) : IAssemblyInitializer
    {
        public unsafe void Initialize(AssemblyInfo info)
        {
            logger.LogDebug("[{tag}] Finalizing assembly initialization...", info.GetTag());

            if (info.VTable.FreeAssemblyVTableDelegate == null)
                throw new InvalidOperationException("Assembly delegate for disposing virtual table descriptor was null");

            info.VTable.FreeAssemblyVTableDelegate(info.VTableDescriptorPointer);

            logger.LogDebug("[{tag}] Assembly initialization finalized", info.GetTag());
        }
    }
}
