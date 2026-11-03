using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Core 
{
    public static unsafe class FreeHttpRequestHandlersMetadataDelegateDescriptorExtensions
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int DescribedFreeHttpRequestHandlersMetadataDelegate(StringDescriptor* descriptor);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void DescribedFreeDelegate();

        extension(FreeHttpRequestHandlersMetadataDelegateDescriptor descriptor)
        {
            public static FreeHttpRequestHandlersMetadataDelegateDescriptor Alloc(
                FreeHttpRequestHandlersMetadataDelegate del)
            {
                var describedHandle = GetDescribedDelegate(del);
                var delegatePointer = Marshal.GetFunctionPointerForDelegate(describedHandle);

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
                    Delegate = (delegate* unmanaged[Cdecl]<StringDescriptor*, int>)delegatePointer,
                    FreeAction = (delegate* unmanaged[Cdecl]<void>)freeDelegatePointer
                };
            }

            public FreeHttpRequestHandlersMetadataDelegate ToManaged()
            {
                var del = descriptor.Delegate;

                if (del == null)
                    throw new InvalidOperationException(
                        $"Unmanaged delegate was null. {nameof(FreeHttpRequestHandlersMetadataDelegateDescriptor)}");

                return GetDelegate(del);
            }
        }

        private static DescribedFreeDelegate AllocateDelegates(
            DescribedFreeHttpRequestHandlersMetadataDelegate describedHandle)
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

        private static DescribedFreeHttpRequestHandlersMetadataDelegate GetDescribedDelegate(
            FreeHttpRequestHandlersMetadataDelegate del)
            => (StringDescriptor* descriptor) =>
            {
                try
                {
                    return del(descriptor);
                }
                catch
                {
                    return 1;
                }
            };

        private static FreeHttpRequestHandlersMetadataDelegate GetDelegate(
            delegate* unmanaged[Cdecl]<StringDescriptor*, int> del)
            => (StringDescriptor* descriptor) =>
            {
                return del(descriptor);
            };
    }
}