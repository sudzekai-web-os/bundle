using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Core 
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct HostVTableDescriptor : IMemFreeable
    {
        public uint AbiVersion;

        public FreeHttpRequestDelegateDescriptor* FreeHttpRequestDelegate;

        public readonly bool IsAllocated => FreeHttpRequestDelegate != null;

        public void Free()
        {
            if (!IsAllocated)
                return;

            AllocationUtilities.DeepFreeCoTaskMemIfNotNull(ref FreeHttpRequestDelegate);
        }
    }
}