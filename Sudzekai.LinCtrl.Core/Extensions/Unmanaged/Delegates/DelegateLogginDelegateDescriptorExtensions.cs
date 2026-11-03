using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Core 
{
    public static unsafe class DelegateLogginDelegateDescriptorExtensions
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int DescribedDelegateLogginDelegate(LogDelegateDescriptor* descriptor);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void DescribedFreeDelegate();

        extension(DelegateLogginDelegateDescriptor descriptor)
        {
            public static DelegateLogginDelegateDescriptor Alloc(DelegateLoggingDelegate del)
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
                    Delegate = (delegate* unmanaged[Cdecl]<LogDelegateDescriptor*, int>)delegatePointer,
                    FreeAction = (delegate* unmanaged[Cdecl]<void>)freeDelegatePointer
                };
            }

            public DelegateLoggingDelegate ToManaged()
            {
                var del = descriptor.Delegate;

                if (del == null)
                    throw new InvalidOperationException(
                        $"Unmanaged delegate was null. {nameof(DelegateLogginDelegateDescriptor)}");

                return GetDelegate(del);
            }
        }

        private static DescribedFreeDelegate AllocateDelegates(
            DescribedDelegateLogginDelegate describedHandle)
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

        private static DescribedDelegateLogginDelegate GetDescribedDelegate(
            DelegateLoggingDelegate del)
            => (LogDelegateDescriptor* logDelegateDescriptor) =>
            {
                try
                {
                    if (logDelegateDescriptor == null)
                        return 1;

                    var logDelegate = logDelegateDescriptor->ToManaged();

                    return del(logDelegate);
                }
                catch
                {
                    return 1;
                }
            };

        private static DelegateLoggingDelegate GetDelegate(
            delegate* unmanaged[Cdecl]<LogDelegateDescriptor*, int> del)
            => (LogDelegate logDelegate) =>
            {
                ArgumentNullException.ThrowIfNull(logDelegate);

                var descriptorPointer = AllocationUtilities.AllocCoTaskMem(
                    LogDelegateDescriptor.Alloc(logDelegate));

                using var descriptorGuard = MemScopeProvider.CreateScope(ref descriptorPointer);

                return del(descriptorPointer);
            };
    }
}