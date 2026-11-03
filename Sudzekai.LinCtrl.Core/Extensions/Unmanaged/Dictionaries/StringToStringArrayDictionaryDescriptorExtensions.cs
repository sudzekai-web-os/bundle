using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Core 
{
    public static unsafe class StringToStringArrayDictionaryDescriptorExtensions
    {
        extension(StringToStringArrayDictionaryDescriptor descriptor)
        {
            public static StringToStringArrayDictionaryDescriptor Alloc(ReadOnlySpan<StringToStringArrayDescriptor> source)
            {
                var count = source.Length;

                if (count == 0)
                    return new()
                    {
                        Pairs = null,
                        Count = 0
                    };

                StringToStringArrayDescriptor* pairsPointer;

                var bytes = checked(count * sizeof(StringToStringArrayDescriptor));
                pairsPointer = (StringToStringArrayDescriptor*)Marshal.AllocCoTaskMem(bytes);

                try
                {
                    source.CopyTo(new Span<StringToStringArrayDescriptor>(pairsPointer, count));
                }
                catch
                {
                    Marshal.FreeCoTaskMem((nint)pairsPointer);
                    throw;
                }

                return new()
                {
                    Pairs = pairsPointer,
                    Count = count
                };
            }

            public static StringToStringArrayDictionaryDescriptor Alloc(Dictionary<string, string[]> source)
            {
                List<StringToStringArrayDescriptor> temp = [];

                try
                {
                    foreach (var pair in source)
                        temp.Add(StringToStringArrayDescriptor.Alloc(pair));
                }
                catch
                {
                    foreach (var pairDescriptor in temp)
                        pairDescriptor.Free();
                    throw;
                }

                return Alloc(CollectionsMarshal.AsSpan(temp));
            }

            public Dictionary<string, string[]> ToManaged()
            {
                if (descriptor.Pairs == null || descriptor.Count <= 0)
                    return [];

                Dictionary<string, string[]> result = [];

                for (int i = 0; i < descriptor.Count; i++)
                {
                    var pair = descriptor.Pairs[i].ToManaged();
                    result[pair.Key] = pair.Value;
                }

                return result;
            }
        }
    }
}