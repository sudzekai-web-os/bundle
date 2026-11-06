using Sudzekai.LinCtrl.Abi.Abstractions;
using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Abi
{
    [StructLayout(LayoutKind.Sequential)]
    public struct HostVTableDescriptor(
        uint abiVersion) : IAllocationState
    {
        public uint AbiVersion = abiVersion;

        public readonly bool IsAllocated => false;
    }
}
