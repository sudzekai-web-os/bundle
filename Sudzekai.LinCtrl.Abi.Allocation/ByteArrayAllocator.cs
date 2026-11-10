using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Abi.Allocation
{
    public static class ByteArrayAllocator
    {
        public unsafe static ByteArrayViewer Alloc(byte[] source)
        {
            ArgumentNullException.ThrowIfNull(source, nameof(source));

            var dest = (byte*)Marshal.AllocCoTaskMem(Math.Max(1, source.Length));

            try
            {
                fixed (byte* src = source)
                    Buffer.MemoryCopy(src, dest, source.Length, source.Length);
            }
            catch
            {
                Marshal.FreeCoTaskMem((nint)dest);
                
                throw;
            }

            return new(dest, source.Length);
        }

        public unsafe static void Free(ref ByteArrayViewer viewer)
        {
            if (!viewer.IsAllocated)
                return;

            Marshal.FreeCoTaskMem((nint)viewer.Array);
            
            viewer = default;
        }
    }
}
