using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Core 
{
    public static unsafe class LogDelegateDescriptorExtensions
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void DescribedLogDelegate(LogEntryDescriptor* descriptor);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void DescribedFreeDelegate();

        extension(LogDelegateDescriptor descriptor)
        {
            public static LogDelegateDescriptor Alloc(LogDelegate del)
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
                    Delegate = (delegate* unmanaged[Cdecl]<LogEntryDescriptor*, void>)delegatePointer,
                    FreeAction = (delegate* unmanaged[Cdecl]<void>)freeDelegatePointer
                };
            }

            public LogDelegate ToManaged()
            {
                var del = descriptor.Delegate;

                if (del == null)
                    throw new InvalidOperationException(
                        $"Unmanaged delegate was null. {nameof(LogDelegateDescriptor)}");

                return GetDelegate(del);
            }
        }

        private static DescribedFreeDelegate AllocateDelegates(
            DescribedLogDelegate describedHandle)
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

        private static DescribedLogDelegate GetDescribedDelegate(
            LogDelegate del)
            => (LogEntryDescriptor* descriptor) =>
            {
                try
                {
                    if (descriptor == null)
                        return;

                    var entry = descriptor->ToManaged();

                    del(entry);
                }
                catch
                {
                }
            };

        private static LogDelegate GetDelegate(
            delegate* unmanaged[Cdecl]<LogEntryDescriptor*, void> del)
            => (LogEntry entry) =>
            {
                ArgumentNullException.ThrowIfNull(entry);

                var descriptorPointer = AllocationUtilities.AllocCoTaskMem(
                    LogEntryDescriptor.Alloc(entry));

                using var descriptorGuard = MemScopeProvider.CreateScope(ref descriptorPointer);

                del(descriptorPointer);
            };
    }
}