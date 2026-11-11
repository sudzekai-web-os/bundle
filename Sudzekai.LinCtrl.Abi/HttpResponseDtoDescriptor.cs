using Sudzekai.LinCtrl.Abi.Abstractions;
using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Abi
{
    [StructLayout(LayoutKind.Sequential)]
    public struct HttpResponseDtoDescriptor(
        int contentType, 
        int statusCode, 
        ByteArrayViewer data,
        int errorCode = 0,
        StringViewer errorMessage = default) : IAllocationState
    {
        public int ContentType = contentType;

        public int StatusCode = statusCode;

        public ByteArrayViewer Data = data;

        public int ErrorCode = errorCode;

        public StringViewer ErrorMessage = errorMessage;

        public readonly bool IsAllocated => Data.IsAllocated || ErrorMessage.IsAllocated;
    }
}
