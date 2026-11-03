using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Core 
{
    public static unsafe class ByteArrayDescriptorExtensions
    {
        extension(ByteArrayDescriptor descriptor)
        {
            public static ByteArrayDescriptor Alloc(ReadOnlySpan<byte> source)
            {
                var count = source.Length;

                if (count == 0)
                    return new() { Array = null, Count = 0 };

                byte* byteArrayPointer;
                byteArrayPointer = (byte*)Marshal.AllocCoTaskMem(count);

                try
                {
                    source.CopyTo(new Span<byte>(byteArrayPointer, count));
                }
                catch
                {
                    Marshal.FreeCoTaskMem((nint)byteArrayPointer);
                    throw;
                }

                return new() { Array = byteArrayPointer, Count = count };
            }

            public static ByteArrayDescriptor Alloc(IEnumerable<byte> source)
            {
                ArgumentNullException.ThrowIfNull(source);

                var temp = new List<byte>(source);

                return Alloc(CollectionsMarshal.AsSpan(temp));
            }

            public byte[] ToManaged()
            {
                if (descriptor.Array == null || descriptor.Count <= 0)
                    return [];

                var result = new byte[descriptor.Count];
                Marshal.Copy((nint)descriptor.Array, result, 0, descriptor.Count);
                return result;
            }
        }
    }
}