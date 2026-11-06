using Sudzekai.LinCtrl.Abi.Abstractions;
using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Abi
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct AssemblyVTableDescriptor(
        int abiVersion, 
        delegate* unmanaged[Cdecl]<int> initDelegate, 
        delegate* unmanaged[Cdecl]<delegate* unmanaged[Cdecl]<LogEntryDtoDescriptor*, int>, int> delegateLoggingDelegate, 
        delegate* unmanaged[Cdecl]<int> buildDelegate, 
        delegate* unmanaged[Cdecl]<RequestHandlerArrayDescriptor*, int> getRequestHandlersDelegate, 
        delegate* unmanaged[Cdecl]<StringViewer*, int> getRequestHandlersMetadataDelegate, 
        delegate* unmanaged[Cdecl]<int> disposeAssemblyDelegate) : IAllocationState
    {
        public int AbiVersion = abiVersion;

        public delegate* unmanaged[Cdecl]<int> InitDelegate = initDelegate;

        public delegate* unmanaged[Cdecl]<delegate* unmanaged[Cdecl]<LogEntryDtoDescriptor*, int>, int> DelegateLoggingDelegate = delegateLoggingDelegate;

        public delegate* unmanaged[Cdecl]<int> BuildDelegate = buildDelegate;

        public delegate* unmanaged[Cdecl]<RequestHandlerArrayDescriptor*, int> GetRequestHandlersDelegate = getRequestHandlersDelegate;

        public delegate* unmanaged[Cdecl]<StringViewer*, int> GetRequestHandlersMetadataDelegate = getRequestHandlersMetadataDelegate;

        public delegate* unmanaged[Cdecl]<int> DisposeAssemblyDelegate = disposeAssemblyDelegate;

        public readonly bool IsAllocated => InitDelegate != null
            || DelegateLoggingDelegate != null
            || BuildDelegate != null
            || GetRequestHandlersDelegate != null
            || GetRequestHandlersMetadataDelegate != null
            || DisposeAssemblyDelegate != null;
    }
}
