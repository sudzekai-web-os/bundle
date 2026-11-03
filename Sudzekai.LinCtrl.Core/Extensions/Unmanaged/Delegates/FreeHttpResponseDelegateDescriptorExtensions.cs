using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Core 
{
    public static unsafe class FreeHttpResponseDelegateDescriptorExtensions
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int DescribedFreeHttpResponseDelegate(HttpResponseDescriptor* descriptor);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void DescribedFreeDelegate();

        extension(FreeHttpResponseDelegateDescriptor descriptor)
        {
            public static FreeHttpResponseDelegateDescriptor Alloc(FreeHttpResponseDelegate del)
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
                    Delegate = (delegate* unmanaged[Cdecl]<HttpResponseDescriptor*, int>)delegatePointer,
                    FreeAction = (delegate* unmanaged[Cdecl]<void>)freeDelegatePointer
                };
            }

            public FreeHttpResponseDelegate ToManaged()
            {
                var del = descriptor.Delegate;

                if (del == null)
                    throw new InvalidOperationException(
                        $"Unmanaged delegate was null. {nameof(FreeHttpResponseDelegateDescriptor)}");

                return GetDelegate(del);
            }
        }

        private static DescribedFreeDelegate AllocateDelegates(
            DescribedFreeHttpResponseDelegate describedHandle)
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

        private static DescribedFreeHttpResponseDelegate GetDescribedDelegate(
            FreeHttpResponseDelegate del)
            => (HttpResponseDescriptor* descriptor) =>
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

        private static FreeHttpResponseDelegate GetDelegate(
            delegate* unmanaged[Cdecl]<HttpResponseDescriptor*, int> del)
            => (HttpResponseDescriptor* descriptor) =>
            {
                return del(descriptor);
            };
    }
}