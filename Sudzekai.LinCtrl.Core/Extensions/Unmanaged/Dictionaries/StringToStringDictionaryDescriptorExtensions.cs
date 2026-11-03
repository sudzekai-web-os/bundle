using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Core 
{
    public static unsafe class StringToStringDictionaryDescriptorExtensions
    {
        extension(StringToStringDictionaryDescriptor descriptor)
        {
            public static StringToStringDictionaryDescriptor Alloc(ReadOnlySpan<StringToStringDescriptor> source)
            {
                var count = source.Length;

                if (count == 0)
                    return new()
                    {
                        Pairs = null,
                        Count = 0
                    };

                StringToStringDescriptor* pairsArrayPointer;

                var bytes = checked(count * sizeof(StringToStringDescriptor));

                pairsArrayPointer = (StringToStringDescriptor*)Marshal.AllocCoTaskMem(bytes);

                try
                {
                    source.CopyTo(new Span<StringToStringDescriptor>(pairsArrayPointer, count));
                }
                catch
                {
                    Marshal.FreeCoTaskMem((nint)pairsArrayPointer);
                    throw;
                }

                return new()
                {
                    Pairs = pairsArrayPointer,
                    Count = count
                };
            }

            public static StringToStringDictionaryDescriptor Alloc(Dictionary<string, string> source)
            {
                List<StringToStringDescriptor> temp = [];

                try
                {
                    foreach (var pair in source)
                        temp.Add(StringToStringDescriptor.Alloc(pair));
                }
                catch
                {
                    foreach (var pairDescriptor in temp)
                        pairDescriptor.Free();

                    throw;
                }

                return Alloc(CollectionsMarshal.AsSpan(temp));
            }

            public Dictionary<string, string> ToManaged()
            {
                if (descriptor.Pairs == null || descriptor.Count <= 0)
                    return [];

                Dictionary<string, string> result = [];

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