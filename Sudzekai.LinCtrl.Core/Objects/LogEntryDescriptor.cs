using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Core 
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct LogEntryDescriptor : IMemFreeable
    {
        public StringDescriptor* Category;

        public StringDescriptor* Message;

        public int LogLevel;

        public readonly bool IsAllocated => Category != null || Message != null;

        public void Free()
        {
            AllocationUtilities.DeepFreeCoTaskMemIfNotNull(ref Category);
            AllocationUtilities.DeepFreeCoTaskMemIfNotNull(ref Message);
        }
    }
}