using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Core 
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct StringToStringArrayDescriptor : IMemFreeable
    {
        public StringDescriptor* Key;

        public StringArrayDescriptor* Value;

        public readonly bool IsAllocated => Key != null || Value != null;

        public void Free()
        {
            AllocationUtilities.DeepFreeCoTaskMemIfNotNull(ref Key);
            AllocationUtilities.DeepFreeCoTaskMemIfNotNull(ref Value);
        }
    }
}