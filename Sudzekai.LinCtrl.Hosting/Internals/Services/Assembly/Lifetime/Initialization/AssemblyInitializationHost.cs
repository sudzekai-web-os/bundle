using Microsoft.Extensions.Logging;

namespace Sudzekai.LinCtrl.Hosting 
{
    internal class AssemblyInitializationHost(
        IEnumerable<IAssemblyInitializationStepHandler> handlers,
        ILogger<AssemblyInitializationHost> logger)
    {
        public void Initialize(AssemblyInfo info)
        {
            logger.LogInformation("Initializing assembly (handle: {Handle})", info.Handle);

            foreach (var handler in handlers)
                handler.Handle(info);

            info.IsInitialized = true;

            logger.LogInformation("Assembly initialized");
        }
    }
}
