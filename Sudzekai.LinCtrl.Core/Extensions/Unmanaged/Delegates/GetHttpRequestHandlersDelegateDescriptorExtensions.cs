using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Core 
{
    public static unsafe class GetHttpRequestHandlersDelegateDescriptorExtensions
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate HttpRequestHandlerArrayDescriptor* DescribedGetHttpRequestHandlerDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void DescribedFreeDelegate();

        extension(GetHttpRequestHandlersDelegateDescriptor descriptor)
        {
            public static GetHttpRequestHandlersDelegateDescriptor Alloc(GetHttpRequestHandlersDelegate del)
            {
                var describedHandle = GetDescribedDelegate(del);
                var getHandlersDelegatePointer = Marshal.GetFunctionPointerForDelegate(describedHandle);

                DescribedFreeDelegate freeDelegate = AllocateDelegates(describedHandle);

                nint freeDelegatePointer;

                try
                {
                    freeDelegatePointer = Marshal.GetFunctionPointerForDelegate(freeDelegate);
                }
                catch
                {
                    freeDelegate();
                    throw;
                }

                return new()
                {
                    Delegate = (delegate* unmanaged[Cdecl]<HttpRequestHandlerArrayDescriptor*>)getHandlersDelegatePointer,
                    FreeAction = (delegate* unmanaged[Cdecl]<void>)freeDelegatePointer
                };
            }

            public GetHttpRequestHandlersDelegate ToManaged()
            {
                var del = descriptor.Delegate;

                if (del == null)
                    throw new InvalidOperationException(
                        $"Unmanaged delegate was null. {nameof(GetHttpRequestHandlersDelegateDescriptor)}");

                return GetDelegate(del);
            }
        }

        private static DescribedFreeDelegate AllocateDelegates(
            DescribedGetHttpRequestHandlerDelegate describedHandle)
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

        private static DescribedGetHttpRequestHandlerDelegate GetDescribedDelegate(
            GetHttpRequestHandlersDelegate getHandlersDelegate)
            => () =>
            {
                try
                {
                    var response = getHandlersDelegate();
                    var arrayDescriptor = HttpRequestHandlerArrayDescriptor.Alloc(response);

                    try
                    {
                        return AllocationUtilities.AllocCoTaskMem(arrayDescriptor);
                    }
                    catch
                    {
                        arrayDescriptor.Free();
                        throw;
                    }
                }
                catch
                {
                    return null;
                }
            };

        private static GetHttpRequestHandlersDelegate GetDelegate(
            delegate* unmanaged[Cdecl]<HttpRequestHandlerArrayDescriptor*> del)
            => () =>
            {
                var responsePointer = del();

                if (responsePointer == null)
                    throw new InvalidOperationException(
                        "Native handler returned a null pointer");

                using var responseGuard = MemScopeProvider.CreateScope(ref responsePointer);

                return responsePointer->ToManaged();
            };
    }
}