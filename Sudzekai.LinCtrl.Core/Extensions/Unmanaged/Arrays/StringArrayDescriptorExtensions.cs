using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Core 
{
    public static unsafe class StringArrayDescriptorExtensions
    {
        extension(StringArrayDescriptor descriptor)
        {
            public static StringArrayDescriptor Alloc(ReadOnlySpan<StringDescriptor> source)
            {
                var count = source.Length;

                if (count == 0)
                    return new()
                    {
                        Array = null,
                        Count = 0
                    };

                StringDescriptor* stringArrayPointer;

                var bytes = checked(count * sizeof(StringDescriptor));
                stringArrayPointer = (StringDescriptor*)Marshal.AllocCoTaskMem(bytes);

                try
                {
                    source.CopyTo(new Span<StringDescriptor>(stringArrayPointer, count));
                }
                catch
                {
                    Marshal.FreeCoTaskMem((nint)stringArrayPointer);
                    throw;
                }

                return new()
                {
                    Array = stringArrayPointer,
                    Count = count
                };
            }

            public static StringArrayDescriptor Alloc(IEnumerable<string> source)
            {
                ArgumentNullException.ThrowIfNull(source);

                List<StringDescriptor> temp = [];

                try
                {
                    foreach (var str in source)
                        temp.Add(StringDescriptor.Alloc(str));
                }
                catch
                {
                    foreach (var strDescriptor in temp)
                        strDescriptor.Free();

                    throw;
                }

                return Alloc(CollectionsMarshal.AsSpan(temp));
            }

            public string[] ToManaged()
            {
                if (descriptor.Array == null || descriptor.Count <= 0)
                    return [];

                var result = new string[descriptor.Count];
                var span = new ReadOnlySpan<StringDescriptor>(descriptor.Array, descriptor.Count);

                for (int i = 0; i < descriptor.Count; i++)
                    result[i] = span[i].ToManaged();

                return result;
            }
        }
    }
}