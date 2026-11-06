using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Abi.Allocation
{
    public static class CoTaskAllocator
    {
        public static unsafe T* Alloc<T>(T source)
            where T : unmanaged
        {
            var ptr = (T*)Marshal.AllocCoTaskMem(sizeof(T));

            *ptr = source;

            return ptr;
        }

        public static unsafe void Free<T>(ref T* ptr)
            where T : unmanaged
        {
            if (ptr == null)
                return;

            Marshal.FreeCoTaskMem((nint)ptr);

            ptr = null;
        }
    }
}
