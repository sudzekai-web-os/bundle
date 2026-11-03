namespace Sudzekai.LinCtrl.Core
{
    public class AssemblyVTable(uint abiVersion)
    {
        public uint AbiVersion { get; } = abiVersion;

		public InitializeDelegate? InitializeDelegate { get; set; }
		
        public DelegateLoggingDelegate? DelegateLoggingDelegate { get; set; }

		public BuildDelegate? BuildDelegate { get; set; }

		public GetHttpRequestHandlersDelegate? GetHttpRequestHandlersDelegate { get; set; }

		public GetHttpRequestHandlersMetadataDelegate? GetHttpRequestHandlersMetadataDelegate { get; set; }

		// dispose delegates

		public FreeHttpRequestHandlersMetadataDelegate? FreeHttpRequestHandlersMetadataDelegate { get; set; }

		public FreeLogEntryDelegate? FreeLogEntryDelegate { get; set; }

		public FreeHttpResponseDelegate? FreeHttpResponseDelegate { get; set; }

		public FreeAssemblyVTableDelegate? FreeAssemblyVTableDelegate { get; set; }
		
		public AssemblyDisposeDelegate? AssemblyDisposeDelegate { get; set; }
	}
}