using Sudzekai.LinCtrl.Abi.Abstractions;
using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Abi
{
    [StructLayout(LayoutKind.Sequential)]
    public struct StringToStringDescriptor(
        StringViewer key,
        StringViewer value) : IAllocationState
    {
        public StringViewer Key = key;

        public StringViewer Value = value;

        public readonly bool IsAllocated => Key.IsAllocated || Value.IsAllocated;
    }
}
