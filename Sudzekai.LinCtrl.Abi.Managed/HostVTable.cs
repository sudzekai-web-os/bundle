using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Abi.Managed
{
    public class HostVTable
    {
        public uint AbiVersion { get; set; } = LinCtrlContext.AbiVersion;
    }
}
