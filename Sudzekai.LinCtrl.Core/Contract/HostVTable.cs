
namespace Sudzekai.LinCtrl.Core 
{
    public class HostVTable(uint abiVersion)
    {
        public uint AbiVersion { get; set; } = abiVersion;
    }
}