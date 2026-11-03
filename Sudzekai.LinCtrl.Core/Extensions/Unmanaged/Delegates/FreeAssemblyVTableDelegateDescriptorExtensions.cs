using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Core 
{
    public static unsafe class FreeAssemblyVTableDelegateDescriptorExtensions
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int DescribedFreeAssemblyVTableDelegate(AssemblyVTableDescriptor* descriptor);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void DescribedFreeDelegate();

        extension(FreeAssemblyVTableDelegateDescriptor descriptor)
        {
            public static FreeAssemblyVTableDelegateDescriptor Alloc(FreeAssemblyVTableDelegate del)
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
                    Delegate = (delegate* unmanaged[Cdecl]<AssemblyVTableDescriptor*, int>)delegatePointer,
                    FreeAction = (delegate* unmanaged[Cdecl]<void>)freeDelegatePointer
                };
            }

            public FreeAssemblyVTableDelegate ToManaged()
            {
                var del = descriptor.Delegate;

                if (del == null)
                    throw new InvalidOperationException(
                        $"Unmanaged delegate was null. {nameof(FreeAssemblyVTableDelegateDescriptor)}");

                return GetDelegate(del);
            }
        }

        private static DescribedFreeDelegate AllocateDelegates(
            DescribedFreeAssemblyVTableDelegate describedHandle)
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

        private static DescribedFreeAssemblyVTableDelegate GetDescribedDelegate(
            FreeAssemblyVTableDelegate del)
            => (AssemblyVTableDescriptor* descriptor) =>
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

        private static FreeAssemblyVTableDelegate GetDelegate(
            delegate* unmanaged[Cdecl]<AssemblyVTableDescriptor*, int> del)
            => (AssemblyVTableDescriptor* descriptor) =>
            {
                return del(descriptor);
            };
    }
}