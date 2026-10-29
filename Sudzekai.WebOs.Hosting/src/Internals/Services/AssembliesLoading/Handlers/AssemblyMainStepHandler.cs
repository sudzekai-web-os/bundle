using Sudzekai.WebOs.Core;
using System.Runtime.InteropServices;

namespace Sudzekai.WebOs.Hosting
{
    internal class AssemblyMainStepHandler : IAssemblyLoadingStepHandler
    {
        public unsafe string Handle(nint lib, PluginAssembly plugin)
        {
            if (!NativeLibrary.TryGetExport(lib, WebOsCoreContext.Exports.Main, out var export))
                throw new InvalidOperationException(string.Format(LogsDictionary.AssemblyExport.DoesntExport, WebOsCoreContext.Exports.Main));

            delegate* unmanaged<int> func;

            try
            {
                func = (delegate* unmanaged<int>)export;
            }
            catch
            {
                throw new InvalidOperationException(string.Format(LogsDictionary.AssemblyExport.InvalidExport, WebOsCoreContext.Exports.Main));
            }

            var result = func();

            if (result != 0)
                throw new InvalidOperationException(LogsDictionary.AssemblyMainStepHandler.InitInternalError);

            return LogsDictionary.AssemblyMainStepHandler.EntryPointProcessed;
        }
    }
}
