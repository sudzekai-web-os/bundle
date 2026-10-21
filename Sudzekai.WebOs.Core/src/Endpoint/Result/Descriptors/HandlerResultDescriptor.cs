using System.Runtime.InteropServices;

namespace Sudzekai.WebOs.Core
{
    [StructLayout(LayoutKind.Sequential)]
    public struct HandlerResultDescriptor : IDisposable
    {
        public int StatusCode;

        public nint Data; public int DataLength;
        
        public int ContentType;

        public HandlerResultDescriptor(int statusCode, nint data, int dataLength, int contentType)
        {
            StatusCode = statusCode;
            Data = data;
            DataLength = dataLength;
            ContentType = contentType;
        }

        public void Dispose()
        {
            AllocationUtilities.FreeIfAllocated(ref Data);
        }
    }
}
