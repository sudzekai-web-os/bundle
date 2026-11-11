using Sudzekai.LinCtrl.Abi.Allocation.Types.Exceptions;
using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Abi.Allocation
{
    public static class StringToStringDictionaryAllocator
    {
        public static unsafe StringToStringDictionaryDescriptor Alloc(ReadOnlySpan<StringToStringDescriptor> source)
        {
            ItemNotAllocatedException.ThrowIfNotAllocated(source);

            var size = checked(source.Length * sizeof(StringToStringDescriptor));

            var dest = (StringToStringDescriptor*)Marshal.AllocCoTaskMem(size);

            try
            {
                fixed (StringToStringDescriptor* src = source)
                    Buffer.MemoryCopy(src, dest, size, size);
            }
            catch
            {
                Marshal.FreeCoTaskMem((nint)dest);
                throw;
            }

            return new(dest, source.Length);
        }

        public static StringToStringDictionaryDescriptor Alloc(IEnumerable<KeyValuePair<string, string>> source)
            => AllocNullableValues(source.Select(entry => new KeyValuePair<string, string?>(entry.Key, entry.Value)));

        internal static StringToStringDictionaryDescriptor AllocNullableValues(IEnumerable<KeyValuePair<string, string?>> source)
        {
            ArgumentNullException.ThrowIfNull(source);

            List<StringToStringDescriptor> tmp = [];

            try
            {
                foreach (var item in source)
                {
                    StringToStringDescriptor descriptor = default;
                    try
                    {
                        descriptor.Key = StringAllocator.Alloc(item.Key);
                        descriptor.Value = item.Value is null ? default : StringAllocator.Alloc(item.Value);
                        tmp.Add(descriptor);
                    }
                    catch
                    {
                        StringToStringAllocator.Free(ref descriptor);
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
                    StringAllocator.Free(ref tmpSpan[i].Value);
                }

                throw;
            }
        }

        public static unsafe void Free(ref StringToStringDictionaryDescriptor descriptor)
        {
            if (!descriptor.IsAllocated)
                return;

            for (int i = 0; i < descriptor.Count; i++)
                StringToStringAllocator.Free(ref descriptor.Array[i]);

            Marshal.FreeCoTaskMem((nint)descriptor.Array);

            descriptor = default;
        }
    }
}
