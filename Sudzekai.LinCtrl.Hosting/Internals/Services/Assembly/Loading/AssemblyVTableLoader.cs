using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Sudzekai.LinCtrl.Abi;
using Sudzekai.LinCtrl.Abi.Allocation;
using Sudzekai.LinCtrl.Core;
using System.Runtime.InteropServices;
using Sudzekai.LinCtrl.Abi.Mapping;

namespace Sudzekai.LinCtrl.Hosting
{
    internal class AssemblyVTableLoader(
        ILogger<AssemblyVTableLoader> logger,
        IConfiguration configuration) : IAssemblyLoader
    {
        public unsafe void Load(AssemblyInfo info)
        {
            var tag = info.GetTag();

            logger.LogDebug("[{tag}] Exchanging virtual tables...", tag);

            var handle = NativeLibraryCache.Load(info.FullPath);

            if (!NativeLibrary.TryGetExport(handle, AssemblyExports.ExchangeVTables, out var getVTableAddress))
                throw new InvalidOperationException($"Assembly not exports \"{AssemblyExports.ExchangeVTables}\"");

            var func = (delegate* unmanaged[Cdecl]<HostVTableDescriptor*, AssemblyVTableDescriptor**, int>)getVTableAddress;
            var hostVTable = HostVTableAllocator.Alloc(LinCtrlContext.AbiVersion, configuration.AsEnumerable());
            AssemblyVTableDescriptor* assemblyVTablePtr = null;

            try
            {
                var result = func(&hostVTable, &assemblyVTablePtr);
                if (result != 0)
                    throw new InvalidOperationException($"Assembly export returned error result code: {result}");
                if (assemblyVTablePtr == null)
                    throw new InvalidOperationException("Assembly export returned null virtual table pointer");

                info.VTable = AssemblyVTableMapper.ToManaged(*assemblyVTablePtr);
            }
            finally
            {
                HostVTableAllocator.Free(ref hostVTable);
                CoTaskAllocator.Free(ref assemblyVTablePtr);
            }

            logger.LogDebug("[{tag}] Virtual tables exchanged", tag);
        }
    }
}
