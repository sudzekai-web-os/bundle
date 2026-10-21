using System.Runtime.InteropServices;

namespace Sudzekai.WebOs.Core
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct HandlerRequestDescriptor : IDisposable
    {
        public int HttpMethod;

        public nint Path; public int PathLength;

        public nint QueryString; public int QueryStringLength;

        public HeaderEntryDescriptor* Headers; public int HeadersCount;

        public nint Body; public int BodyLength;

        public void Dispose()
        {
            AllocationUtilities.FreeIfAllocated(ref Path);
            AllocationUtilities.FreeIfAllocated(ref QueryString);
            AllocationUtilities.FreeIfAllocated(ref Body);

            FreeHeaders(Headers, HeadersCount);
        }

        private static void FreeHeaders(HeaderEntryDescriptor* headers, int count)
        {
            if (headers == null)
                return;

            for (int i = 0; i < count; i++)
                (&headers[i])->Dispose(); // обнуление полей реального элемента

            Marshal.FreeCoTaskMem((nint)headers);
        }
    }
}
