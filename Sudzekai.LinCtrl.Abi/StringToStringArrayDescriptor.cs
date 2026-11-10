using Sudzekai.LinCtrl.Abi.Abstractions;
using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Abi
{
    [StructLayout(LayoutKind.Sequential)]
    public struct StringToStringArrayDescriptor(
        StringViewer key, 
        StringArrayDescriptor value) : IAllocationState
    {
        public StringViewer Key = key;

        public StringArrayDescriptor Value = value;

        public readonly bool IsAllocated => Key.IsAllocated || Value.IsAllocated;
    }
}
