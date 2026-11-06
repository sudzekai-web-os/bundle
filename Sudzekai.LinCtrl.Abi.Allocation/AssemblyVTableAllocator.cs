namespace Sudzekai.LinCtrl.Abi.Allocation
{
    public static class AssemblyVTableAllocator
    {
        public static unsafe AssemblyVTableDescriptor Alloc(
            int abiVersion,
            delegate* unmanaged[Cdecl]<int> initDelegate,
            delegate* unmanaged[Cdecl]<delegate* unmanaged[Cdecl]<LogEntryDtoDescriptor*, int>, int> delegateLoggingDelegate,
            delegate* unmanaged[Cdecl]<int> buildDelegate,
            delegate* unmanaged[Cdecl]<RequestHandlerArrayDescriptor*, int> getRequestHandlersDelegate,
            delegate* unmanaged[Cdecl]<StringViewer*, int> getRequestHandlersMetadataDelegate,
            delegate* unmanaged[Cdecl]<int> disposeAssemblyDelegate)
            => new(abiVersion,
                initDelegate,
                delegateLoggingDelegate,
                buildDelegate,
                getRequestHandlersDelegate,
                getRequestHandlersMetadataDelegate,
                disposeAssemblyDelegate);

        public static void Free(ref AssemblyVTableDescriptor descriptor)
        {
            if (!descriptor.IsAllocated)
                return;
        }
    }
}
