using Microsoft.Extensions.Logging;

namespace Sudzekai.LinCtrl.Hosting 
{
    internal class AssemblyDisposingHost(
        IEnumerable<IAssemblyDisposingStepHandler> handlers,
        ILogger<AssemblyDisposingHost> logger)
    {
        public void Dispose(AssemblyInfo info)
        {
            logger.LogInformation("Disposing assembly (handle: {Handle})", info.Handle);

            foreach (var handler in handlers)
                handler.Handle(info);

            info.IsDisposed = true;

            logger.LogInformation("Assembly disposed");
        }
    }
}
