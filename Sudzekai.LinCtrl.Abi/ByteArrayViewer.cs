using Sudzekai.LinCtrl.Abi.Abstractions;
using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Abi
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ByteArrayViewer(
        byte* array, 
        int count) : IAllocationState
    {
        public byte* Array = array;

        public int Count = count;

        public readonly bool IsAllocated => Array != null;
    }
}
