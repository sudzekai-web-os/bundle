using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Core 
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct AssemblyVTableDescriptor : IMemFreeable
    {
        public uint AbiVersion;

        public GetHttpRequestHandlersDelegateDescriptor* GetHttpRequestHandlersDelegate;

        public FreeAssemblyVTableDelegateDescriptor* FreeAssemblyVTableDelegate;

        public GetHttpRequestHandlersMetadataDelegateDescriptor* GetHttpRequestHandlersMetadataDelegate;

        public AssemblyDisposeDelegateDescriptor* AssemblyDisposeDelegate;

        public InitializeDelegateDescriptor* InitializeDelegate;

        public BuildDelegateDescriptor* BuildDelegate;

        public DelegateLogginDelegateDescriptor* DelegateLogginDelegate;

        public readonly bool IsAllocated =>
            GetHttpRequestHandlersDelegate != null
            || FreeAssemblyVTableDelegate != null
            || GetHttpRequestHandlersMetadataDelegate != null
            || AssemblyDisposeDelegate != null
            || InitializeDelegate != null
            || BuildDelegate != null
            || DelegateLogginDelegate != null;

        public void Free()
        {
            AllocationUtilities.DeepFreeCoTaskMemIfNotNull(ref GetHttpRequestHandlersDelegate);
            AllocationUtilities.DeepFreeCoTaskMemIfNotNull(ref FreeAssemblyVTableDelegate);
            AllocationUtilities.DeepFreeCoTaskMemIfNotNull(ref GetHttpRequestHandlersMetadataDelegate);
            AllocationUtilities.DeepFreeCoTaskMemIfNotNull(ref AssemblyDisposeDelegate);
            AllocationUtilities.DeepFreeCoTaskMemIfNotNull(ref InitializeDelegate);
            AllocationUtilities.DeepFreeCoTaskMemIfNotNull(ref BuildDelegate);
            AllocationUtilities.DeepFreeCoTaskMemIfNotNull(ref DelegateLogginDelegate);
        }
    }
}