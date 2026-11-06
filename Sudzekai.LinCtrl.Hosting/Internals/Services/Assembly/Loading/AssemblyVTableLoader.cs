using Microsoft.Extensions.Logging;
using Sudzekai.LinCtrl.Abi;
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

            logger.LogDebug("[{tag}] Exchanging virtual tables...", tag);

            nint handle;

            handle = NativeLibrary.Load(info.FullPath);

            info.Handle = handle;

            if (!NativeLibrary.TryGetExport(handle, AssemblyExports.ExchangeVTables, out var getVTableAddress))
                throw new InvalidOperationException($"Assembly not exports \"{AssemblyExports.ExchangeVTables}\"");

            var func = (delegate* unmanaged[Cdecl]<HostVTableDescriptor*, AssemblyVTableDescriptor**, int>)getVTableAddress;

            AssemblyVTableDescriptor* assemblyVTablePtr = null;

            var result = func(HostingContext.VTableDescriptorPointer, &assemblyVTablePtr);

            if (result != 0)
                throw new InvalidOperationException($"Assembly export \"{AssemblyExports.ExchangeVTables}\" returned error result code: {result}");
            
            if (assemblyVTablePtr == null)
                throw new InvalidOperationException($"Assembly export \"{AssemblyExports.ExchangeVTables}\" returned null virtual table pointer");

            var vTable = assemblyVTablePtr->ToManaged();

            info.VTable = vTable;
            info.VTableDescriptorPointer = assemblyVTablePtr;

            logger.LogDebug("[{tag}] Virtual tables exchanged", tag);
        }
    }
}
