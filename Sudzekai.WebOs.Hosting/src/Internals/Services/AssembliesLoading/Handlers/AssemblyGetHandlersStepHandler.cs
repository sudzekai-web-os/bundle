using Sudzekai.WebOs.Core;
using System.Runtime.InteropServices;

namespace Sudzekai.WebOs.Hosting
{
    internal class AssemblyGetHandlersStepHandler : IAssemblyLoadingStepHandler
    {
        public unsafe string Handle(nint lib, PluginAssembly plugin)
        {
            if (!NativeLibrary.TryGetExport(lib, WebOsCoreContext.Exports.GetHandlers, out var export))
                throw new InvalidOperationException(string.Format(LogsDictionary.AssemblyExport.DoesntExport, WebOsCoreContext.Exports.GetHandlers));

            delegate* unmanaged<int*, EndpointHandlerDescriptor*> func;

            try
            {
                func = (delegate* unmanaged<int*, EndpointHandlerDescriptor*>)export;
            }
            catch
            {
                throw new InvalidOperationException(string.Format(LogsDictionary.AssemblyExport.InvalidExport, WebOsCoreContext.Exports.GetHandlers));
            }

            var count = 0;

            var mem = func(&count);

            for (int i = 0; i < count; i++)
                plugin.EndpointHandlers = [.. plugin.EndpointHandlers.Append(EndpointHandler.FromDescriptor(ref mem[i]))];

            return LogsDictionary.AssemblyGetHandlersStepHandler.HandlersReceived;
        }
    }
}
