using System.Runtime.InteropServices;

namespace Sudzekai.WebOs.Core.src.Endpoint.Request.Descriptors
{
    [StructLayout(LayoutKind.Sequential)]
    public struct StringEntryDescriptor
    {
        public nint Key; public int KeyLength;

        public nint Value; public int ValueLength;

        public void Dispose()
        {
            AllocationUtilities.FreeIfAllocated(ref Key);
            AllocationUtilities.FreeIfAllocated(ref Value);
        }
    }
}
