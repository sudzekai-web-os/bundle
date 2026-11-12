using Sudzekai.LinCtrl.Abi.Allocation.Types.Exceptions;

namespace Sudzekai.LinCtrl.Abi.Allocation
{
    public static class RequestHandlerAllocator
    {
        public static unsafe RequestHandlerDescriptor Alloc(
            StringViewer pattern,
            int httpMethod,
            delegate* unmanaged[Cdecl]<HttpRequestDtoDescriptor*, HttpResponseDtoDescriptor*, int> handleDelegate,
            int isFiltering = 0)
        {
            ArgumentNotAllocatedException.ThrowIfNotAllocated(pattern, nameof(pattern));

            return new(pattern, httpMethod, handleDelegate, isFiltering);
        }

        public static void Free(ref RequestHandlerDescriptor descriptor)
        {
            if (!descriptor.IsAllocated)
                return;

            StringAllocator.Free(ref descriptor.Pattern);

            descriptor = default;
        }
    }
}
