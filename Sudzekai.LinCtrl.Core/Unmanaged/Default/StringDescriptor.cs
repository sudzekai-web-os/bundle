using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Core 
{
    [StructLayout(LayoutKind.Sequential)]
    public struct StringDescriptor : IMemFreeable
    {
        public nint Pointer;

        public int Length;

        public readonly bool IsAllocated => Pointer != nint.Zero;

        public void Free()
        {
            if (!IsAllocated)
                return;

            Marshal.FreeCoTaskMem(Pointer);

            Pointer = nint.Zero;
            Length = 0;
        }
    }
}