using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Core 
{
    public static unsafe class HttpRequestHandlerArrayDescriptorExtensions
    {
        extension(HttpRequestHandlerArrayDescriptor descriptor)
        {
            public static HttpRequestHandlerArrayDescriptor Alloc(ReadOnlySpan<HttpRequestHandlerDescriptor> source)
            {
                var count = source.Length;

                if (count == 0)
                    return new()
                    {
                        Array = null,
                        Count = 0
                    };

                HttpRequestHandlerDescriptor* handlerArrayPointer;

                var bytes = checked(count * sizeof(HttpRequestHandlerDescriptor));
                handlerArrayPointer = (HttpRequestHandlerDescriptor*)Marshal.AllocCoTaskMem(bytes);

                try
                {
                    source.CopyTo(new Span<HttpRequestHandlerDescriptor>(handlerArrayPointer, count));
                }
                catch
                {
                    Marshal.FreeCoTaskMem((nint)handlerArrayPointer);
                    throw;
                }

                return new()
                {
                    Array = handlerArrayPointer,
                    Count = count
                };
            }

            public static HttpRequestHandlerArrayDescriptor Alloc(IEnumerable<IHttpRequestHandler> source)
            {
                ArgumentNullException.ThrowIfNull(source);

                List<HttpRequestHandlerDescriptor> temp = [];

                try
                {
                    foreach (var handler in source)
                        temp.Add(HttpRequestHandlerDescriptor.Alloc(handler));
                }
                catch
                {
                    foreach (var handlerDescriptor in temp)
                        handlerDescriptor.Free();

                    throw;
                }

                return Alloc(CollectionsMarshal.AsSpan(temp));
            }

            public IHttpRequestHandler[] ToManaged()
            {
                if (descriptor.Array == null || descriptor.Count <= 0)
                    return [];

                var result = new IHttpRequestHandler[descriptor.Count];
                var span = new ReadOnlySpan<HttpRequestHandlerDescriptor>(descriptor.Array, descriptor.Count);

                for (int i = 0; i < descriptor.Count; i++)
                    result[i] = span[i].ToManaged();

                return result;
            }
        }
    }
}