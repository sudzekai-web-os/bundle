using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Core 
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct StringToStringArrayDictionaryDescriptor : IMemFreeable
    {
        public StringToStringArrayDescriptor* Pairs;

        public int Count;

        public readonly bool IsAllocated => Pairs != null;

        public void Free()
        {
            if (!IsAllocated)
                return;

            AllocationUtilities.DeepFreeCoTaskMemIfNotNull(ref Pairs);

            Count = 0;
        }
    }
}