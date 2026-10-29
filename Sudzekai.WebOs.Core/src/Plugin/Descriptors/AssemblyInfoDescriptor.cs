using System.Runtime.InteropServices;

namespace Sudzekai.WebOs.Core
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AssemblyInfoDescriptor : IDisposable
    {
        public nint Name; public int NameLength;

        public nint Version; public int VersionLength;

        public nint Summary; public int SummaryLength;

        public void Dispose()
        {
            AllocationUtilities.FreeIfAllocated(ref Name);
            AllocationUtilities.FreeIfAllocated(ref Version);
            AllocationUtilities.FreeIfAllocated(ref Summary);
        }
    }
}
