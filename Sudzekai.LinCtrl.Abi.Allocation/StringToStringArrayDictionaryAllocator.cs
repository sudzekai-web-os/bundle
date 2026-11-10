using Sudzekai.LinCtrl.Abi.Allocation.Types.Exceptions;
using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Abi.Allocation
{
    public static class StringToStringArrayDictionaryAllocator
    {
        public static unsafe StringToStringArrayDictionaryDescriptor Alloc(ReadOnlySpan<StringToStringArrayDescriptor> source)
        {
            ItemNotAllocatedException.ThrowIfNotAllocated(source);

            var size = checked(source.Length * sizeof(StringToStringArrayDescriptor));

            var dest = (StringToStringArrayDescriptor*)Marshal.AllocCoTaskMem(size);

            try
            {
                fixed (StringToStringArrayDescriptor* src = source)
                    Buffer.MemoryCopy(src, dest, size, size);
            }
            catch
            {
                Marshal.FreeCoTaskMem((nint)dest);
                throw;
            }

            return new(dest, source.Length);
        }

        public static StringToStringArrayDictionaryDescriptor Alloc(IEnumerable<KeyValuePair<string, string[]>> source)
        {
            ArgumentNullException.ThrowIfNull(source, nameof(source));

            List<StringToStringArrayDescriptor> tmp = [];

            try
            {
                foreach (var item in source)
                {
                    StringToStringArrayDescriptor descriptor = default;
                    try
                    {
                        descriptor.Key = StringAllocator.Alloc(item.Key);
                        descriptor.Value = StringArrayAllocator.Alloc(item.Value);
                        tmp.Add(descriptor);
                    }
                    catch
                    {
                        StringToStringArrayAllocator.Free(ref descriptor);
                        throw;
                    }
                }

                return Alloc(CollectionsMarshal.AsSpan(tmp));
            }
            catch
            {
                var tmpSpan = CollectionsMarshal.AsSpan(tmp);

                for (int i = 0; i < tmpSpan.Length; i++)
                {
                    StringAllocator.Free(ref tmpSpan[i].Key);
                    StringArrayAllocator.Free(ref tmpSpan[i].Value);
                }

                throw;
            }
        }

        public static StringToStringArrayDictionaryDescriptor Alloc(IEnumerable<KeyValuePair<string, IEnumerable<string>>> source)
        {
            ArgumentNullException.ThrowIfNull(source, nameof(source));

            return Alloc(source.Select(p => new KeyValuePair<string, string[]>(p.Key, [.. p.Value])));
        }

        public static unsafe void Free(ref StringToStringArrayDictionaryDescriptor descriptor)
        {
            if (!descriptor.IsAllocated)
                return;

            for (int i = 0; i < descriptor.Count; i++)
                StringToStringArrayAllocator.Free(ref descriptor.Array[i]);

            Marshal.FreeCoTaskMem((nint)descriptor.Array);

            descriptor = default;
        }
    }
}
