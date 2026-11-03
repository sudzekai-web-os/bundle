using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Core 
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct HttpResponseDescriptor : IMemFreeable
    {
        public int StatusCode;

        public ByteArrayDescriptor* Data;

        public int ContentType;

        public readonly bool IsAllocated => Data != null;

        public void Free()
        {
            AllocationUtilities.DeepFreeCoTaskMemIfNotNull(ref Data);
        }
    }
}