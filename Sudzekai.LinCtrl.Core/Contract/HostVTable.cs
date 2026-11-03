
namespace Sudzekai.LinCtrl.Core 
{
    public unsafe class HostVTable(uint abiVersion)
    {
        public uint AbiVersion { get; set; } = abiVersion;

        public FreeHttpRequestDelegate? FreeHttpRequestDelegate { get; set; }
    }
}