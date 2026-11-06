using Sudzekai.LinCtrl.Abi.Abstractions;
using System.Net;
using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Abi
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct StringToStringDictionaryDescriptor(
        StringToStringDescriptor* array,
        int count) : IAllocationState
    {
        public StringToStringDescriptor* Array = array;

        public int Count = count;

        public readonly bool IsAllocated => Array != null;
    }
}
