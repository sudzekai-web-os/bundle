using System.Net;
using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Core
{
    public static unsafe class RequestHandleDelegateDescriptorExtensions
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate HttpResponseDescriptor* DescribedRequestHandleDelegate(HttpRequestDescriptor* request);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void DescribedFreeDelegate();

        extension(RequestHandleDelegateDescriptor descriptor)
        {
            public static RequestHandleDelegateDescriptor Alloc(RequestHandleDelegate del)
            {
                var describedHandle = GetDescribedDelegate(del);
                var handlePointer = Marshal.GetFunctionPointerForDelegate(describedHandle);

                DescribedFreeDelegate freeDelegate = AllocateDelegates(describedHandle);

                nint freePointer;

                try
                {
                    freePointer = Marshal.GetFunctionPointerForDelegate(freeDelegate);
                }
                catch
                {
                    freeDelegate();
                    throw;
                }

                return new()
                {
                    Delegate = (delegate* unmanaged[Cdecl]<HttpRequestDescriptor*, HttpResponseDescriptor*>)handlePointer,
                    FreeAction = (delegate* unmanaged[Cdecl]<void>)freePointer
                };
            }

            public RequestHandleDelegate ToManaged()
            {
                var del = descriptor.Delegate;

                if (del == null)
                    throw new InvalidOperationException(
                        $"Unmanaged delegate was null. {nameof(RequestHandleDelegateDescriptor)}");

                return GetDelegate(del);
            }
        }

        private static DescribedFreeDelegate AllocateDelegates(
            DescribedRequestHandleDelegate describedHandle)
        {
            GCHandle handleDelegateHandle = default;
            GCHandle freeDelegateHandle = default;

            DescribedFreeDelegate freeDelegate = () =>
            {
                GCHandleCallbackDictionary.Free(ref handleDelegateHandle);
                GCHandleCallbackDictionary.Free(ref freeDelegateHandle);
            };

            try
            {
                handleDelegateHandle = GCHandleCallbackDictionary.Alloc(describedHandle);
                freeDelegateHandle = GCHandleCallbackDictionary.Alloc(freeDelegate);
            }
            catch
            {
                GCHandleCallbackDictionary.Free(ref handleDelegateHandle);
                GCHandleCallbackDictionary.Free(ref freeDelegateHandle);
                throw;
            }

            return freeDelegate;
        }

        private static DescribedRequestHandleDelegate GetDescribedDelegate(
            RequestHandleDelegate handleDelegate)
            => descriptor =>
            {
                try
                {
                    if (descriptor == null)
                        return AllocateErrorResponse(1);

                    var request = descriptor->ToManaged();

                    var response = handleDelegate(request).GetAwaiter().GetResult();

                    if (response == null)
                        return AllocateErrorResponse(2);

                    var descriptorResult = HttpResponseDescriptor.Alloc(response);

                    try
                    {
                        return AllocationUtilities.AllocCoTaskMem(descriptorResult);
                    }
                    catch
                    {
                        descriptorResult.Free();
                        throw;
                    }
                }
                catch
                {
                    return AllocateErrorResponse(3);
                }
            };

        private static HttpResponseDescriptor* AllocateErrorResponse(int code)
        {
            try
            {
                var errored = new HttpResponse(HttpStatusCode.InternalServerError)
                    .WithData($"Internal server error while executing request. Internal code: {code})")
                    .As(ContentType.ApplicationJson);

                return AllocationUtilities.AllocCoTaskMem(
                    HttpResponseDescriptor.Alloc(errored));
            }
            catch
            {
                return null;
            }
        }

        private static RequestHandleDelegate GetDelegate(
            delegate* unmanaged[Cdecl]<HttpRequestDescriptor*, HttpResponseDescriptor*> del)
            => request =>
            {
                ArgumentNullException.ThrowIfNull(request);

                var requestPointer = AllocationUtilities.AllocCoTaskMem(
                    HttpRequestDescriptor.Alloc(request));

                using var requestGuard = MemScopeProvider.CreateScope(ref requestPointer);

                var responsePointer = del(requestPointer);

                if (responsePointer == null)
                    throw new InvalidOperationException(
                        "Native handler returned a null pointer");

                using var responseGuard = MemScopeProvider.CreateScope(ref responsePointer);

                return Task.FromResult(responsePointer->ToManaged());
            };
    }
}