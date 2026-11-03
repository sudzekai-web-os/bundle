using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Core 
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct StringArrayDescriptor : IMemFreeable
    {
        public StringDescriptor* Array;

        public int Count;

        public readonly bool IsAllocated => Array != null;

        public void Free()
        {
            if (!IsAllocated)
                return;

            AllocationUtilities.DeepFreeCoTaskMemIfNotNull(ref Array);

            Count = 0;
        }
    }
}