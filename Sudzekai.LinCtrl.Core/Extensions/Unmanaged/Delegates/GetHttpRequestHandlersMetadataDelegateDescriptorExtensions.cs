using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Core 
{
    public static unsafe class GetHttpRequestHandlersMetadataDelegateDescriptorExtensions
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate StringDescriptor* DescribedGetHttpRequestHandlersMetadataDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void DescribedFreeDelegate();

        extension(GetHttpRequestHandlersMetadataDelegateDescriptor descriptor)
        {
            public static GetHttpRequestHandlersMetadataDelegateDescriptor Alloc(
                GetHttpRequestHandlersMetadataDelegate del)
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
                    Delegate = (delegate* unmanaged[Cdecl]<StringDescriptor*>)getHandlersDelegatePointer,
                    FreeAction = (delegate* unmanaged[Cdecl]<void>)freeDelegatePointer
                };
            }

            public GetHttpRequestHandlersMetadataDelegate ToManaged()
            {
                var del = descriptor.Delegate;

                if (del == null)
                    throw new InvalidOperationException(
                        $"Unmanaged delegate was null. {nameof(GetHttpRequestHandlersMetadataDelegateDescriptor)}");

                return GetDelegate(del);
            }
        }

        private static DescribedFreeDelegate AllocateDelegates(
            DescribedGetHttpRequestHandlersMetadataDelegate describedHandle)
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

        private static DescribedGetHttpRequestHandlersMetadataDelegate GetDescribedDelegate(
            GetHttpRequestHandlersMetadataDelegate getHandlersMetadataDelegate)
            => () =>
            {
                try
                {
                    var str = getHandlersMetadataDelegate();
                    var stringDescriptor = StringDescriptor.Alloc(str);

                    try
                    {
                        return AllocationUtilities.AllocCoTaskMem(stringDescriptor);
                    }
                    catch
                    {
                        stringDescriptor.Free();
                        throw;
                    }
                }
                catch
                {
                    return null;
                }
            };

        private static GetHttpRequestHandlersMetadataDelegate GetDelegate(
            delegate* unmanaged[Cdecl]<StringDescriptor*> del)
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