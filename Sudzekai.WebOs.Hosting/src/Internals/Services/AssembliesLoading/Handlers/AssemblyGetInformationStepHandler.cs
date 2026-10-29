using Sudzekai.WebOs.Core;
using System.Runtime.InteropServices;

namespace Sudzekai.WebOs.Hosting
{
    internal class AssemblyGetInformationStepHandler : IAssemblyLoadingStepHandler
    {
        public unsafe string Handle(nint lib, PluginAssembly plugin)
        {
            plugin.Info = new();

            if (!NativeLibrary.TryGetExport(lib, WebOsCoreContext.Exports.GetInfo, out var export))
                throw new InvalidOperationException(string.Format(LogsDictionary.AssemblyExport.DoesntExport, WebOsCoreContext.Exports.GetInfo));

            delegate* unmanaged<AssemblyInfoDescriptor> func;

            try
            {
                func = (delegate* unmanaged<AssemblyInfoDescriptor>)export;
            }
            catch
            {
                throw new InvalidOperationException(string.Format(LogsDictionary.AssemblyExport.InvalidExport, WebOsCoreContext.Exports.GetInfo));
            }

            var descriptor = func();

            plugin.Info = AssemblyInfo.FromDescriptor(ref descriptor);

            return LogsDictionary.AssemblyGetInformationStepHandler.InfoReceived;
        }
    }
}
