using Sudzekai.WebOs.Core;
using System.Runtime.InteropServices;

namespace Sudzekai.WebOs.Hosting
{
    internal class AssemblyGetHandlersMetadataStepHandler : IAssemblyLoadingStepHandler
    {
        public unsafe string Handle(nint lib, PluginAssembly plugin)
        {
            if (!NativeLibrary.TryGetExport(lib, WebOsCoreContext.Exports.GetHandlersMetadata, out var export))
                throw new InvalidOperationException(string.Format(LogsDictionary.AssemblyExport.DoesntExport, WebOsCoreContext.Exports.GetHandlersMetadata));

            delegate* unmanaged<int*, nint> func;

            try
            {
                func = (delegate* unmanaged<int*, nint>)export;
            }
            catch
            {
                throw new InvalidOperationException(string.Format(LogsDictionary.AssemblyExport.InvalidExport, WebOsCoreContext.Exports.GetHandlersMetadata));
            }

            int length = 0;

            var ptr = func(&length);

            plugin.HandlersMetadataJson = AllocationUtilities.ToString(ptr, length);

            Marshal.FreeCoTaskMem(ptr);

            return LogsDictionary.AssemblyGetHandlersMetadataStepHandler.HandlersMetadataReceived;
        }
    }
}
