using Sudzekai.LinCtrl.Abi.Abstractions;
using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Abi
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct RequestHandlerDescriptor(
        StringViewer pattern, 
        int httpMethod, 
        delegate* unmanaged[Cdecl]<HttpRequestDtoDescriptor*, HttpResponseDtoDescriptor*, int> handleDelegate,
        int isFiltering = 0) : IAllocationState
    {
        public StringViewer Pattern = pattern;

        public int HttpMethod = httpMethod;

        public delegate* unmanaged[Cdecl]<HttpRequestDtoDescriptor*, HttpResponseDtoDescriptor*, int> HandleDelegate = handleDelegate;

        public int IsFiltering = isFiltering;

        public readonly bool IsAllocated => Pattern.IsAllocated || HandleDelegate != null;
    }
}
