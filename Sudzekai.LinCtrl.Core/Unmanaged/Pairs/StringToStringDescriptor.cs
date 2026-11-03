using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Core 
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct StringToStringDescriptor : IMemFreeable
    {
        public StringDescriptor* Key;

        public StringDescriptor* Value;

        public readonly bool IsAllocated => Key != null || Value != null;

        public void Free()
        {
            AllocationUtilities.DeepFreeCoTaskMemIfNotNull(ref Key);
            AllocationUtilities.DeepFreeCoTaskMemIfNotNull(ref Value);
        }
    }
}