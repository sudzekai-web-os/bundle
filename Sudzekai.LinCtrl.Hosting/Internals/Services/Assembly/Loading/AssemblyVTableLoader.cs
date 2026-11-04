using Microsoft.Extensions.Logging;
using Sudzekai.LinCtrl.Core;
using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Hosting
{
    internal class AssemblyVTableLoader(
        ILogger<AssemblyVTableLoader> logger) : IAssemblyLoader
    {
        public unsafe void Load(AssemblyInfo info)
        {
            var tag = info.GetTag();

            logger.LogDebug("[{tag}] Receiving virtual table...", tag);

            nint handle;

            handle = NativeLibrary.Load(info.FullPath);

            info.Handle = handle;

            if (!NativeLibrary.TryGetExport(handle, AssemblyExports.GetVTable, out var getVTableAddress))
                throw new InvalidOperationException($"Assembly not exports \"{AssemblyExports.GetVTable}\"");

            var func = (delegate* unmanaged[Cdecl]<AssemblyVTableDescriptor*>)getVTableAddress;

            var vTableDescriptorPtr = func();

            if (vTableDescriptorPtr == null)
                throw new InvalidOperationException($"Assembly export \"{AssemblyExports.GetVTable}\" returned null pointer");

            var vTable = vTableDescriptorPtr->ToManaged();

            info.VTable = vTable;
            info.VTableDescriptorPointer = vTableDescriptorPtr;

            logger.LogDebug("[{tag}] Virtual table received", tag);
        }
    }
}
