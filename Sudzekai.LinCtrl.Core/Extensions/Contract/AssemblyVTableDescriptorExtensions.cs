
namespace Sudzekai.LinCtrl.Core 
{
    public static unsafe class AssemblyVTableDescriptorExtensions
    {
        extension(AssemblyVTableDescriptor descriptor)
        {
            public static AssemblyVTableDescriptor Alloc(
                uint abiVersion,
                GetHttpRequestHandlersDelegateDescriptor getHttpRequestHandlersDelegateDescriptor,
                FreeAssemblyVTableDelegateDescriptor freeAssemblyVTableDelegateDescriptor)
            {
                GetHttpRequestHandlersDelegateDescriptor* getHttpRequestHandlersDelegateDescriptorPointer = null;
                FreeAssemblyVTableDelegateDescriptor* freeAssemblyVTableDelegateDescriptorPointer = null;

                try
                {
                    getHttpRequestHandlersDelegateDescriptorPointer = AllocationUtilities.AllocCoTaskMem(getHttpRequestHandlersDelegateDescriptor);
                    freeAssemblyVTableDelegateDescriptorPointer = AllocationUtilities.AllocCoTaskMem(freeAssemblyVTableDelegateDescriptor);
                }
                catch
                {
                    AllocationUtilities.FreeCoTaskMemIfNotNull(ref getHttpRequestHandlersDelegateDescriptorPointer);
                    AllocationUtilities.FreeCoTaskMemIfNotNull(ref freeAssemblyVTableDelegateDescriptorPointer);

                    throw;
                }

                return new()
                {
                    AbiVersion = abiVersion,
                    GetHttpRequestHandlersDelegate = getHttpRequestHandlersDelegateDescriptorPointer
                };
            }

            public static AssemblyVTableDescriptor Alloc(AssemblyVTable source)
            {
                ArgumentNullException.ThrowIfNull(source.GetHttpRequestHandlersDelegate, nameof(source.GetHttpRequestHandlersDelegate));
                ArgumentNullException.ThrowIfNull(source.FreeAssemblyVTableDelegate, nameof(source.FreeAssemblyVTableDelegate));

                ArgumentNullException.ThrowIfNull(source.GetHttpRequestHandlersMetadataDelegate, nameof(source.GetHttpRequestHandlersMetadataDelegate));
                ArgumentNullException.ThrowIfNull(source.FreeHttpRequestHandlersMetadataDelegate, nameof(source.FreeHttpRequestHandlersMetadataDelegate));
                ArgumentNullException.ThrowIfNull(source.AssemblyDisposeDelegate, nameof(source.AssemblyDisposeDelegate));
                ArgumentNullException.ThrowIfNull(source.InitializeDelegate, nameof(source.InitializeDelegate));
                ArgumentNullException.ThrowIfNull(source.BuildDelegate, nameof(source.BuildDelegate));
                ArgumentNullException.ThrowIfNull(source.DelegateLoggingDelegate, nameof(source.DelegateLoggingDelegate));
                ArgumentNullException.ThrowIfNull(source.FreeLogEntryDelegate, nameof(source.FreeLogEntryDelegate));
                ArgumentNullException.ThrowIfNull(source.FreeHttpResponseDelegate, nameof(source.FreeHttpResponseDelegate));

                GetHttpRequestHandlersDelegateDescriptor getHttpRequestHandlersDelegateDescriptor = default;
                FreeAssemblyVTableDelegateDescriptor freeAssemblyVTableDelegateDescriptor = default;

                GetHttpRequestHandlersMetadataDelegateDescriptor getHttpRequestHandlersMetadataDelegateDescriptor = default;
                FreeHttpRequestHandlersMetadataDelegateDescriptor freeHttpRequestHandlersMetadataDelegateDescriptor = default;
                AssemblyDisposeDelegateDescriptor assemblyDisposeDelegateDescriptor = default;
                InitializeDelegateDescriptor initializeDelegateDescriptor = default;
                BuildDelegateDescriptor buildDelegateDescriptor = default;
                DelegateLogginDelegateDescriptor delegateLogginDelegateDescriptor = default;
                FreeLogEntryDelegateDescriptor freeLogEntryDelegateDescriptor = default;
                FreeHttpResponseDelegateDescriptor freeHttpResponseDelegateDescriptor = default;

                try
                {
                    getHttpRequestHandlersDelegateDescriptor = GetHttpRequestHandlersDelegateDescriptor.Alloc(source.GetHttpRequestHandlersDelegate);
                    freeAssemblyVTableDelegateDescriptor = FreeAssemblyVTableDelegateDescriptor.Alloc(source.FreeAssemblyVTableDelegate);

                    getHttpRequestHandlersMetadataDelegateDescriptor = GetHttpRequestHandlersMetadataDelegateDescriptor.Alloc(source.GetHttpRequestHandlersMetadataDelegate);
                    freeHttpRequestHandlersMetadataDelegateDescriptor = FreeHttpRequestHandlersMetadataDelegateDescriptor.Alloc(source.FreeHttpRequestHandlersMetadataDelegate);
                    assemblyDisposeDelegateDescriptor = AssemblyDisposeDelegateDescriptor.Alloc(source.AssemblyDisposeDelegate);
                    initializeDelegateDescriptor = InitializeDelegateDescriptor.Alloc(source.InitializeDelegate);
                    buildDelegateDescriptor = BuildDelegateDescriptor.Alloc(source.BuildDelegate);
                    delegateLogginDelegateDescriptor = DelegateLogginDelegateDescriptor.Alloc(source.DelegateLoggingDelegate);
                    freeLogEntryDelegateDescriptor = FreeLogEntryDelegateDescriptor.Alloc(source.FreeLogEntryDelegate);
                    freeHttpResponseDelegateDescriptor = FreeHttpResponseDelegateDescriptor.Alloc(source.FreeHttpResponseDelegate);

                    var result = Alloc(source.AbiVersion, getHttpRequestHandlersDelegateDescriptor, freeAssemblyVTableDelegateDescriptor);

                    result.GetHttpRequestHandlersMetadataDelegate = AllocationUtilities.AllocCoTaskMem(getHttpRequestHandlersMetadataDelegateDescriptor);
                    result.FreeHttpRequestHandlersMetadataDelegate = AllocationUtilities.AllocCoTaskMem(freeHttpRequestHandlersMetadataDelegateDescriptor);
                    result.AssemblyDisposeDelegate = AllocationUtilities.AllocCoTaskMem(assemblyDisposeDelegateDescriptor);
                    result.InitializeDelegate = AllocationUtilities.AllocCoTaskMem(initializeDelegateDescriptor);
                    result.BuildDelegate = AllocationUtilities.AllocCoTaskMem(buildDelegateDescriptor);
                    result.DelegateLogginDelegate = AllocationUtilities.AllocCoTaskMem(delegateLogginDelegateDescriptor);
                    result.FreeLogEntryDelegate = AllocationUtilities.AllocCoTaskMem(freeLogEntryDelegateDescriptor);
                    result.FreeHttpResponseDelegate = AllocationUtilities.AllocCoTaskMem(freeHttpResponseDelegateDescriptor);

                    return result;
                }
                catch
                {
                    getHttpRequestHandlersDelegateDescriptor.Free();

                    throw;
                }
            }

            public AssemblyVTable ToManaged()
            {
                var getHttpRequestHandlersDelegate = descriptor.GetHttpRequestHandlersDelegate == null
                    ? null : descriptor.GetHttpRequestHandlersDelegate->ToManaged();

                var getHttpRequestHandlersMetadataDelegate = descriptor.GetHttpRequestHandlersMetadataDelegate == null
                    ? null : descriptor.GetHttpRequestHandlersMetadataDelegate->ToManaged();

                var freeHttpRequestHandlersMetadataDelegate = descriptor.FreeHttpRequestHandlersMetadataDelegate == null
                    ? null : descriptor.FreeHttpRequestHandlersMetadataDelegate->ToManaged();

                var assemblyDisposeDelegate = descriptor.AssemblyDisposeDelegate == null
                    ? null : descriptor.AssemblyDisposeDelegate->ToManaged();

                var initializeDelegate = descriptor.InitializeDelegate == null
                    ? null : descriptor.InitializeDelegate->ToManaged();

                var buildDelegate = descriptor.BuildDelegate == null
                    ? null : descriptor.BuildDelegate->ToManaged();

                var delegateLogginDelegate = descriptor.DelegateLogginDelegate == null
                    ? null : descriptor.DelegateLogginDelegate->ToManaged();

                var freeLogEntryDelegate = descriptor.FreeLogEntryDelegate == null
                    ? null : descriptor.FreeLogEntryDelegate->ToManaged();

                var freeHttpResponseDelegate = descriptor.FreeHttpResponseDelegate == null
                    ? null : descriptor.FreeHttpResponseDelegate->ToManaged();

                return new(descriptor.AbiVersion)
                {
                    GetHttpRequestHandlersDelegate = getHttpRequestHandlersDelegate,
                    GetHttpRequestHandlersMetadataDelegate = getHttpRequestHandlersMetadataDelegate,
                    FreeHttpRequestHandlersMetadataDelegate = freeHttpRequestHandlersMetadataDelegate,
                    AssemblyDisposeDelegate = assemblyDisposeDelegate,
                    InitializeDelegate = initializeDelegate,
                    BuildDelegate = buildDelegate,
                    DelegateLoggingDelegate = delegateLogginDelegate,
                    FreeLogEntryDelegate = freeLogEntryDelegate,
                    FreeHttpResponseDelegate = freeHttpResponseDelegate
                };
            }
        }
    }
}