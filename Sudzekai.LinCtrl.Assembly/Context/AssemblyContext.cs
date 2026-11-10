using Sudzekai.LinCtrl.Abi;
using Sudzekai.LinCtrl.Abi.Allocation;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Abi.Managed.Types.Delegates;
using Sudzekai.LinCtrl.Abi.Mapping;
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Assembly.Context
{
    public class AssemblyContext
    {
        private AssemblyVTableRegistration? _registration;

        public bool IsDisposed => _registration?.IsDisposed ?? false;

        public unsafe AssemblyContext(HostVTableDescriptor* hostVTableDescriptorPointer)
        {
            if (hostVTableDescriptorPointer == null || hostVTableDescriptorPointer->AbiVersion != LinCtrlContext.AbiVersion)
                throw new InvalidOperationException("Host ABI version is not supported");
        }

        public InitDelegate InitDelegate { get; init; } = () => { };

        public DelegateLoggingDelegate DelegateLoggingDelegate { get; init; } = _ => { };

        public BuildDelegate BuildDelegate { get; init; } = () => { };

        public GetRequestHandlersDelegate GetRequestHandlersDelegate { get; init; } = () => [];

        public GetRequestHandlersMetadataDelegate GetRequestHandlersMetadataDelegate { get; init; } = () => string.Empty;

        public AssemblyDisposeDelegate AssemblyDisposeDelegate { get; init; } = () => { };

        public unsafe AssemblyVTableDescriptor* GetVtbl()
        {
            if (_registration != null)
                throw new InvalidOperationException("Assembly virtual table already created");

            _registration = new(new AssemblyVTable
            {
                AbiVersion = (int)LinCtrlContext.AbiVersion,
                InitDelegate = InitDelegate,
                DelegateLoggingDelegate = DelegateLoggingDelegate,
                BuildDelegate = BuildDelegate,
                GetRequestHandlersDelegate = GetRequestHandlersDelegate,
                GetRequestHandlersMetadataDelegate = GetRequestHandlersMetadataDelegate,
                AssemblyDisposeDelegate = AssemblyDisposeDelegate
            });

            return CoTaskAllocator.Alloc(_registration.ToDescriptor());
        }
    }
}
