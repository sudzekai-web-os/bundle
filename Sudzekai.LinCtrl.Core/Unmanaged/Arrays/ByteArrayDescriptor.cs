using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Core 
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ByteArrayDescriptor : IMemFreeable
    {
        public byte* Array;

        public int Count;

        public readonly bool IsAllocated => Array != null;

        public void Free()
        {
            if (!IsAllocated)
                return;

            Marshal.FreeCoTaskMem((nint)Array);

            Array = null;
            Count = 0;
        }
    }
}