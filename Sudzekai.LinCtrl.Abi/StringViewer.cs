using Sudzekai.LinCtrl.Abi.Abstractions;
using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Abi
{
    [StructLayout(LayoutKind.Sequential)]
    public struct StringViewer(
        nint address,
        int length) : IAllocationState
    {
        public nint Address = address;

        public int Length = length;

        public readonly bool IsAllocated => Address != nint.Zero;
    }
}
