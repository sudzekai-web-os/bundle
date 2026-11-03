
namespace Sudzekai.LinCtrl.Core 
{
    public static unsafe class StringToStringArrayDescriptorExtensions
    {
        extension(StringToStringArrayDescriptor descriptor)
        {
            public static StringToStringArrayDescriptor Alloc(StringDescriptor key, StringArrayDescriptor value)
            {
                StringDescriptor* keyPointer = null;
                StringArrayDescriptor* valuePointer = null;

                try
                {
                    keyPointer = AllocationUtilities.AllocCoTaskMem(key);
                    valuePointer = AllocationUtilities.AllocCoTaskMem(value);
                }
                catch
                {
                    AllocationUtilities.FreeCoTaskMemIfNotNull(ref keyPointer);
                    AllocationUtilities.FreeCoTaskMemIfNotNull(ref valuePointer);
                    throw;
                }

                return new()
                {
                    Key = keyPointer,
                    Value = valuePointer
                };
            }

            public static StringToStringArrayDescriptor Alloc(string key, IEnumerable<string> value)
            {
                StringDescriptor keyDescriptor = default;
                StringArrayDescriptor valueDescriptor = default;

                try
                {
                    keyDescriptor = StringDescriptor.Alloc(key);
                    valueDescriptor = StringArrayDescriptor.Alloc(value);

                    return Alloc(keyDescriptor, valueDescriptor);
                }
                catch
                {
                    keyDescriptor.Free();
                    valueDescriptor.Free();

                    throw;
                }
            }

            public static StringToStringArrayDescriptor Alloc(string key, ReadOnlySpan<StringDescriptor> value)
            {
                StringDescriptor keyDescriptor = default;
                StringArrayDescriptor valueDescriptor = default;

                try
                {
                    keyDescriptor = StringDescriptor.Alloc(key);
                    valueDescriptor = StringArrayDescriptor.Alloc(value);

                    return Alloc(keyDescriptor, valueDescriptor);
                }
                catch
                {
                    keyDescriptor.Free();
                    valueDescriptor.Free();

                    throw;
                }
            }

            public static StringToStringArrayDescriptor Alloc(KeyValuePair<string, string[]> pair)
                => Alloc(pair.Key, pair.Value);

            public KeyValuePair<string, string[]> ToManaged()
            {
                string key = string.Empty;
                if (descriptor.Key != null)
                    key = descriptor.Key->ToManaged();

                string[] value = [];
                if (descriptor.Value != null)
                    value = descriptor.Value->ToManaged();

                return new(key, value);
            }
        }
    }
}