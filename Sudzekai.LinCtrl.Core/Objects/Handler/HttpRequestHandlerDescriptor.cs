using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Core 
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct HttpRequestHandlerDescriptor : IMemFreeable
    {
        public StringDescriptor* Pattern;

        public int HttpMethod;

        public RequestHandleDelegateDescriptor* Handle;

        public readonly bool IsAllocated => Pattern != null || Handle != null;

        public void Free()
        {
            if (!IsAllocated)
                return;

            AllocationUtilities.DeepFreeCoTaskMemIfNotNull(ref Pattern);
            AllocationUtilities.DeepFreeCoTaskMemIfNotNull(ref Handle);
        }
    }
}