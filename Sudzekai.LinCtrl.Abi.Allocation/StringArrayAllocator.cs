using Sudzekai.LinCtrl.Abi.Allocation.Types.Exceptions;
using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Abi.Allocation
{
    public static class StringArrayAllocator
    {
        public static unsafe StringArrayDescriptor Alloc(ReadOnlySpan<StringViewer> source)
        {
            ItemNotAllocatedException.ThrowIfNotAllocated(source);

            var size = checked(source.Length * sizeof(StringViewer));
            
            var dest = (StringViewer*)Marshal.AllocCoTaskMem(size);

            try
            {
                fixed (StringViewer* src = source)
                    Buffer.MemoryCopy(src, dest, source.Length, source.Length);
            }
            catch
            {
                Marshal.FreeCoTaskMem((nint)dest);

                throw;
            }

            return new(dest, source.Length);
        }

        public static unsafe StringArrayDescriptor Alloc(string[] source)
        {
            ArgumentNullException.ThrowIfNull(source);

            var count = source.Length;
            if (count == 0)
                return default;

            var bytes = checked(count * sizeof(StringViewer));
            var dest = (StringViewer*)Marshal.AllocCoTaskMem(bytes);

            var written = 0;

            try
            {
                for (int i = 0; i < count; i++)
                {
                    dest[i] = StringAllocator.Alloc(source[i]);
                    written++;
                }
            }
            catch
            {
                for (int i = 0; i < written; i++)
                    StringAllocator.Free(ref dest[i]);

                Marshal.FreeCoTaskMem((nint)dest);
                throw;
            }

            return new(dest, count);
        }

        public static StringArrayDescriptor Alloc(IEnumerable<string> source)
            => Alloc([..source]);

        public static unsafe void Free(ref StringArrayDescriptor descriptor)
        {
            if (descriptor.Array == null)
                return;

            for (int i = 0; i < descriptor.Count; i++)
                StringAllocator.Free(ref descriptor.Array[i]);

            Marshal.FreeCoTaskMem((nint)descriptor.Array);

            descriptor = default;
        }
    }
}
