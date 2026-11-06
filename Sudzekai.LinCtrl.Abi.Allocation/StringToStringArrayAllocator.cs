using Sudzekai.LinCtrl.Abi.Allocation.Types.Exceptions;

namespace Sudzekai.LinCtrl.Abi.Allocation
{
    public static class StringToStringArrayAllocator
    {
        public static StringToStringArrayDescriptor Alloc(StringViewer key, StringArrayDescriptor value)
        {
            ArgumentNotAllocatedException.ThrowIfNotAllocated(key, nameof(key));
            ArgumentNotAllocatedException.ThrowIfNotAllocated(value, nameof(value));

            return new(key, value);
        }

        public static StringToStringArrayDescriptor Alloc(string key, string[] value)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(key, nameof(key));

            ArgumentNullException.ThrowIfNull(value, nameof(value));

            StringViewer keyViewer = default;
            StringArrayDescriptor valueDescriptor = default;

            try
            {
                keyViewer = StringAllocator.Alloc(key);
                valueDescriptor = StringArrayAllocator.Alloc(value);

                return Alloc(keyViewer, valueDescriptor);
            }
            catch
            {
                StringAllocator.Free(ref keyViewer);
                StringArrayAllocator.Free(ref valueDescriptor);

                throw;
            }
        }

        public static StringToStringArrayDescriptor Alloc(string key, IEnumerable<string> value)
            => Alloc(key, [..value]);

        public static StringToStringArrayDescriptor Alloc(KeyValuePair<string, string[]> pair)
            => Alloc(pair.Key, pair.Value);

        public static StringToStringArrayDescriptor Alloc(KeyValuePair<string, IEnumerable<string>> pair)
            => Alloc(pair.Key, pair.Value);

        public static void Free(ref StringToStringArrayDescriptor descriptor)
        {
            if (!descriptor.IsAllocated)
                return;

            StringAllocator.Free(ref descriptor.Key);
            StringArrayAllocator.Free(ref descriptor.Value);

            descriptor = default;
        }
    }
}
