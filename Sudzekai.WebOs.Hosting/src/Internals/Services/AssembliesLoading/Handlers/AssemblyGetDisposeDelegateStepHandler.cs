using Sudzekai.WebOs.Core;
using System.Runtime.InteropServices;

namespace Sudzekai.WebOs.Hosting
{
    internal class AssemblyGetDisposeDelegateStepHandler : IAssemblyLoadingStepHandler
    {
        public unsafe string Handle(nint lib, PluginAssembly plugin)
        {
            if (!NativeLibrary.TryGetExport(lib, WebOsCoreContext.Exports.Dispose, out var export))
                throw new InvalidOperationException(string.Format(LogsDictionary.AssemblyExport.DoesntExport, WebOsCoreContext.Exports.Dispose));

            delegate* unmanaged<int> func;

            try
            {
                func = (delegate* unmanaged<int>)export;
            }
            catch
            {
                throw new InvalidOperationException(string.Format(LogsDictionary.AssemblyExport.InvalidExport, WebOsCoreContext.Exports.Dispose));
            }

            plugin.DisposeAction = () =>
            {
                var result = func();

                if (result != 0)
                    throw new InvalidOperationException(string.Format(LogsDictionary.AssemblyGetDisposeDelegateStepHandler.DisposeInternalError));
            };

            return LogsDictionary.AssemblyGetDisposeDelegateStepHandler.DisposeFuncReceived;
        }
    }
}
