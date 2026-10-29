using Sudzekai.WebOs.Core;
using System.Runtime.InteropServices;

namespace Sudzekai.WebOs.Hosting
{
    internal class AssemblyBuildStepHandler : IAssemblyLoadingStepHandler
    {
        public unsafe string Handle(nint lib, PluginAssembly plugin)
        {
            if (!NativeLibrary.TryGetExport(lib, WebOsCoreContext.Exports.Build, out var export))
                throw new InvalidOperationException(string.Format(LogsDictionary.AssemblyExport.DoesntExport, WebOsCoreContext.Exports.Build));

            delegate* unmanaged<int> func;

            try
            {
                func = (delegate* unmanaged<int>)export;
            }
            catch
            {
                throw new InvalidOperationException(string.Format(LogsDictionary.AssemblyExport.InvalidExport, WebOsCoreContext.Exports.Build));
            }

            var result = func();

            if (result != 0)
                throw new InvalidOperationException(LogsDictionary.AssemblyBuildStepHandler.BuildInternalError);

            return LogsDictionary.AssemblyBuildStepHandler.PluginBuilt;
        }
    }
}
