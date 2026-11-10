using Sudzekai.LinCtrl.Abi.Abstractions;
using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Abi
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LogEntryDtoDescriptor(
        StringViewer category,
        StringViewer message,
        int logLevel) : IAllocationState
    {
        public StringViewer Category = category;

        public StringViewer Message = message;

        public int LogLevel = logLevel;

        public readonly bool IsAllocated => Category.IsAllocated || Message.IsAllocated;
    }
}
