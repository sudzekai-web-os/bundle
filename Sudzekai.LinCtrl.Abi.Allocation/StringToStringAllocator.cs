using Sudzekai.LinCtrl.Abi.Allocation.Types.Exceptions;

namespace Sudzekai.LinCtrl.Abi.Allocation
{
    public static class StringToStringAllocator
    {
        public static StringToStringDescriptor Alloc(StringViewer key, StringViewer value)
        {
            ArgumentNotAllocatedException.ThrowIfNotAllocated(key, nameof(key));
            ArgumentNotAllocatedException.ThrowIfNotAllocated(value, nameof(value));

            return new(key, value);
        }

        public static StringToStringDescriptor Alloc(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            StringViewer keyDescriptor = default;
            StringViewer valueDescriptor = default;

            try
            {
                keyDescriptor = StringAllocator.Alloc(key);
                valueDescriptor = StringAllocator.Alloc(value);

                return Alloc(keyDescriptor, valueDescriptor);
            }
            catch
            {
                StringAllocator.Free(ref keyDescriptor);
                StringAllocator.Free(ref valueDescriptor);

                throw;
            }
        }

        public static StringToStringDescriptor Alloc(KeyValuePair<string, string> pair)
            => Alloc(pair.Key, pair.Value);

        public static void Free(ref StringToStringDescriptor descriptor)
        {
            if (!descriptor.IsAllocated)
                return;

            StringAllocator.Free(ref descriptor.Key);
            StringAllocator.Free(ref descriptor.Value);

            descriptor = default;
        }
    }
}
