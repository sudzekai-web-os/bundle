using Microsoft.Extensions.Logging;
using Sudzekai.LinCtrl.Core;
using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Hosting
{
    internal unsafe class AssemblyLoader(
        AssemblyVTableValidator validator,
        ILogger<AssemblyLoader> logger)
    {
        public AssemblyInfo Load(string fullName)
        {
            logger.LogInformation("Loading assembly {fullName}", fullName);

            if (!File.Exists(fullName))
                throw new FileNotFoundException($"Assembly file not found. (fullName = {fullName})");

            var handle = NativeLibrary.Load(fullName);

            var descriptorPtr = GetVTableDescriptor(handle);

            AssemblyVTable vTable = GetVTable(handle, descriptorPtr);

            validator.ThrowIfInvalid(vTable);

            return new(vTable, descriptorPtr, handle);
        }

        private static AssemblyVTable GetVTable(nint handle, AssemblyVTableDescriptor* descriptorPtr)
        {
            AssemblyVTable vTable;
            try
            {
                vTable = descriptorPtr->ToManaged();
            }
            catch
            {
                try
                {
                    var free = descriptorPtr->FreeAssemblyVTableDelegate->Delegate;
                    free(descriptorPtr);
                }
                catch { }

                NativeLibrary.Free(handle);
                throw;
            }

            return vTable;
        }

        private static AssemblyVTableDescriptor* GetVTableDescriptor(nint handle)
        {
            if (!NativeLibrary.TryGetExport(handle, AssemblyExports.GetVTable, out var address))
            {
                NativeLibrary.Free(handle);
                throw new InvalidOperationException(
                    $"Assembly doesn't export {AssemblyExports.GetVTable}");
            }

            var getVTable = (delegate* unmanaged[Cdecl]<AssemblyVTableDescriptor*>)address;
            var descriptorPtr = getVTable();

            if (descriptorPtr == null)
            {
                NativeLibrary.Free(handle);
                throw new InvalidOperationException("Native GetVTable returned null");
            }

            return descriptorPtr;
        }
    }
}
