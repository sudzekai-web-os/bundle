using Sudzekai.LinCtrl.Abi.Abstractions;
using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Abi
{
    [StructLayout(LayoutKind.Sequential)]
    public struct HostVTableDescriptor(
        uint abiVersion,
        StringToStringDictionaryDescriptor configuration = default) : IAllocationState
    {
        public uint AbiVersion = abiVersion;

        public StringToStringDictionaryDescriptor Configuration = configuration;

        public readonly bool IsAllocated => Configuration.IsAllocated;
    }
}
