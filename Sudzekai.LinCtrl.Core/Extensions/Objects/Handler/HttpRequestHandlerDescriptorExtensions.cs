
namespace Sudzekai.LinCtrl.Core 
{
    public static unsafe class HttpRequestHandlerDescriptorExtensions
    {
        extension(HttpRequestHandlerDescriptor descriptor)
        {
            public static HttpRequestHandlerDescriptor Alloc(
                StringDescriptor pattern,
                int httpMethod,
                RequestHandleDelegateDescriptor handleDelegate)
            {
                StringDescriptor* patternPointer = null;
                RequestHandleDelegateDescriptor* handleDelegatePointer = null;

                try
                {
                    patternPointer = AllocationUtilities.AllocCoTaskMem(pattern);
                    handleDelegatePointer = AllocationUtilities.AllocCoTaskMem(handleDelegate);
                }
                catch
                {
                    AllocationUtilities.FreeCoTaskMemIfNotNull(ref patternPointer);
                    AllocationUtilities.FreeCoTaskMemIfNotNull(ref handleDelegatePointer);

                    throw;
                }

                return new()
                {
                    Pattern = patternPointer,
                    Handle = handleDelegatePointer,
                    HttpMethod = httpMethod
                };
            }

            public static HttpRequestHandlerDescriptor Alloc(
                string pattern,
                RequestHandlerHttpMethod httpMethod,
                RequestHandleDelegate requestHandleDelegate)
            {
                StringDescriptor patternDescriptor = default;
                RequestHandleDelegateDescriptor requestHandleDelegateDescriptor = default;

                try
                {
                    patternDescriptor = StringDescriptor.Alloc(pattern);
                    requestHandleDelegateDescriptor = RequestHandleDelegateDescriptor.Alloc(requestHandleDelegate);

                    return Alloc(patternDescriptor, (int)httpMethod, requestHandleDelegateDescriptor);
                }
                catch
                {
                    patternDescriptor.Free();
                    requestHandleDelegateDescriptor.Free();

                    throw;
                }
            }

            public static HttpRequestHandlerDescriptor Alloc(IHttpRequestHandler handler)
                => Alloc(handler.Pattern, handler.HttpMethod, handler.HandleAsync);

            public IHttpRequestHandler ToManaged()
            {
                if (descriptor.Pattern == null)
                    throw new InvalidOperationException("Pattern pointer was null");

                if (descriptor.Handle == null)
                    throw new InvalidOperationException("Handle pointer was null");

                return new HttpRequestHandler(
                    descriptor.Pattern->ToManaged(),
                    (RequestHandlerHttpMethod)descriptor.HttpMethod,
                    descriptor.Handle->ToManaged());
            }
        }
    }
}