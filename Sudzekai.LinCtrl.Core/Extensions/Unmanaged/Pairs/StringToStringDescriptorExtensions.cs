
namespace Sudzekai.LinCtrl.Core 
{
    public static unsafe class StringToStringDescriptorExtensions
    {
        extension(StringToStringDescriptor descriptor)
        {
            public static StringToStringDescriptor Alloc(StringDescriptor key, StringDescriptor value)
            {
                StringDescriptor* keyPointer = null;
                StringDescriptor* valuePointer = null;

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

            public static StringToStringDescriptor Alloc(string key, string value)
            {
                StringDescriptor keyDescriptor = default;
                StringDescriptor valueDescriptor = default;

                try
                {
                    keyDescriptor = StringDescriptor.Alloc(key);
                    valueDescriptor = StringDescriptor.Alloc(value);

                    return Alloc(keyDescriptor, valueDescriptor);
                }
                catch
                {
                    keyDescriptor.Free();
                    valueDescriptor.Free();

                    throw;
                }
            }

            public static StringToStringDescriptor Alloc(KeyValuePair<string, string> pair)
                => Alloc(pair.Key, pair.Value);

            public KeyValuePair<string, string> ToManaged()
            {
                string key = string.Empty;
                if (descriptor.Key != null)
                    key = descriptor.Key->ToManaged();

                string value = string.Empty;
                if (descriptor.Value != null)
                    value = descriptor.Value->ToManaged();

                return new(key, value);
            }
        }
    }
}