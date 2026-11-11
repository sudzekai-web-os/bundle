using Microsoft.Extensions.Configuration;
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

        private IConfiguration? _configuration;

        public IConfiguration Configuration
            => _configuration ?? throw new ObjectDisposedException(nameof(AssemblyContext));

        public bool IsDisposed => _registration?.IsDisposed ?? false;

        public unsafe AssemblyContext(HostVTableDescriptor* hostVTableDescriptorPointer)
        {
            if (hostVTableDescriptorPointer == null || hostVTableDescriptorPointer->AbiVersion != LinCtrlContext.AbiVersion)
                throw new InvalidOperationException("Host ABI version is not supported");

            var descriptor = hostVTableDescriptorPointer->Configuration;
            var values = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < descriptor.Count; i++)
            {
                var entry = descriptor.Array[i];
                values.Add(StringMapper.ToManaged(entry.Key),
                    entry.Value.IsAllocated ? StringMapper.ToManaged(entry.Value) : null);
            }

            _configuration = new ConfigurationBuilder().AddInMemoryCollection(values).Build();
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
                AssemblyDisposeDelegate = DisposeAssembly
            });

            return CoTaskAllocator.Alloc(_registration.ToDescriptor());
        }

        private void DisposeAssembly()
        {
            try
            {
                AssemblyDisposeDelegate();
            }
            finally
            {
                var configuration = _configuration;
                _configuration = null;
                (configuration as IDisposable)?.Dispose();
            }
        }
    }
}
