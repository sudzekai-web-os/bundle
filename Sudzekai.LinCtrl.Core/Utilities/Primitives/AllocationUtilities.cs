using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Core 
{
    public unsafe static class AllocationUtilities
    {
        public static void DeepFreeCoTaskMemIfNotNull<TDescriptor>(ref TDescriptor* descriptor)
            where TDescriptor : unmanaged, IMemFreeable
        {
            if (descriptor != null)
            {
                if (descriptor->IsAllocated)
                    descriptor->Free();

                Marshal.FreeCoTaskMem((nint)descriptor);
                descriptor = null;
            }
        }

        public static void FreeCoTaskMemIfNotNull<TDescriptor>(ref TDescriptor* descriptor)
            where TDescriptor : unmanaged, IMemFreeable
        {
            if (descriptor != null)
            {
                Marshal.FreeCoTaskMem((nint)descriptor);
                descriptor = null;
            }
        }

        public static T* AllocCoTaskMem<T>(T value)
            where T : unmanaged
        {
            var ptr = (T*)Marshal.AllocCoTaskMem(sizeof(T));
            *ptr = value;
            return ptr;
        }
    }
}
