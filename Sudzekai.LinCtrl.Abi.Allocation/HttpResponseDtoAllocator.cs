using Sudzekai.LinCtrl.Abi.Allocation.Types.Exceptions;

namespace Sudzekai.LinCtrl.Abi.Allocation
{
    public static class HttpResponseDtoAllocator
    {
        public static HttpResponseDtoDescriptor Alloc(
            int contentType,
            int statusCode,
            ByteArrayViewer data)
        {
            ArgumentNotAllocatedException.ThrowIfNotAllocated(data, nameof(data));

            return new(contentType, statusCode, data);
        }

        public static HttpResponseDtoDescriptor Alloc(
            int contentType,
            int statusCode,
            byte[] data)
        {
            ArgumentNullException.ThrowIfNull(data);

            ByteArrayViewer dataViewer = default;

            try
            {
                dataViewer = ByteArrayAllocator.Alloc(data);

                return Alloc(contentType, statusCode, dataViewer);
            }
            catch
            {
                ByteArrayAllocator.Free(ref dataViewer);

                throw;
            }
        }

        public static void Free(ref HttpResponseDtoDescriptor descriptor)
        {
            if (!descriptor.IsAllocated)
                return;

            ByteArrayAllocator.Free(ref descriptor.Data);

            descriptor = default;
        }
    }
}
