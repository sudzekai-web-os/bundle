using Sudzekai.LinCtrl.Abi.Abstractions;
using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Abi
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct StringArrayDescriptor(
        StringViewer* array,
        int count) : IAllocationState
    {
        public StringViewer* Array = array;

        public int Count = count;

        public readonly bool IsAllocated => Array != null;
    }
}