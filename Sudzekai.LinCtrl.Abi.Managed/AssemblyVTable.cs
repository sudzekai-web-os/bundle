using Sudzekai.LinCtrl.Abi.Managed.Types.Delegates;

using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Abi.Managed
{
    public class AssemblyVTable
    {
        public int AbiVersion { get; set; } = (int)LinCtrlContext.AbiVersion;

        public InitDelegate InitDelegate { get; set; } = () 
            => { throw new InvalidOperationException($"{nameof(InitDelegate)} not configured"); };
        
        public DelegateLoggingDelegate DelegateLoggingDelegate { get; set; } = (_) 
            => { throw new InvalidOperationException($"{nameof(DelegateLoggingDelegate)} not configured"); };
        
        public BuildDelegate BuildDelegate { get; set; } = () 
            => { throw new InvalidOperationException($"{nameof(BuildDelegate)} not configured"); };
        
        public GetRequestHandlersDelegate GetRequestHandlersDelegate { get; set; } = () 
            => { throw new InvalidOperationException($"{nameof(GetRequestHandlersDelegate)} not configured"); };
        
        public GetRequestHandlersMetadataDelegate GetRequestHandlersMetadataDelegate { get; set; } = () 
            => string.Empty;
        
        public AssemblyDisposeDelegate AssemblyDisposeDelegate { get; set; } = () 
            => { throw new InvalidOperationException($"{nameof(AssemblyDisposeDelegate)} not configured"); };
    }
}
