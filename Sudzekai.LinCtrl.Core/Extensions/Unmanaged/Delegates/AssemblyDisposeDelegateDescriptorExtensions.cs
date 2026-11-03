using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Core 
{
    public static unsafe class AssemblyDisposeDelegateDescriptorExtensions
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int DescribedAssemblyDisposeDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void DescribedFreeDelegate();

        extension(AssemblyDisposeDelegateDescriptor descriptor)
        {
            public static AssemblyDisposeDelegateDescriptor Alloc(AssemblyDisposeDelegate del)
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
                    Delegate = (delegate* unmanaged[Cdecl]<int>)delegatePointer,
                    FreeAction = (delegate* unmanaged[Cdecl]<void>)freeDelegatePointer
                };
            }

            public AssemblyDisposeDelegate ToManaged()
            {
                var del = descriptor.Delegate;

                if (del == null)
                    throw new InvalidOperationException(
                        $"Unmanaged delegate was null. {nameof(AssemblyDisposeDelegateDescriptor)}");

                return GetDelegate(del);
            }
        }

        private static DescribedFreeDelegate AllocateDelegates(
            DescribedAssemblyDisposeDelegate describedHandle)
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

        private static DescribedAssemblyDisposeDelegate GetDescribedDelegate(
            AssemblyDisposeDelegate del)
            => () =>
            {
                try
                {
                    return del();
                }
                catch
                {
                    return 1;
                }
            };

        private static AssemblyDisposeDelegate GetDelegate(
            delegate* unmanaged[Cdecl]<int> del)
            => () =>
            {
                return del();
            };
    }
}