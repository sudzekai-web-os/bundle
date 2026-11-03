using System.Net;

namespace Sudzekai.LinCtrl.Core 
{
    public static unsafe class HttpResponseDescriptorExtensions
    {
        extension(HttpResponseDescriptor descriptor)
        {
            public static HttpResponseDescriptor Alloc(
                int statusCode,
                ByteArrayDescriptor data,
                int contentType)
            {
                ByteArrayDescriptor* dataPointer = null;

                try
                {
                    dataPointer = AllocationUtilities.AllocCoTaskMem(data);
                }
                catch
                {
                    AllocationUtilities.FreeCoTaskMemIfNotNull(ref dataPointer);
                    throw;
                }

                return new()
                {
                    StatusCode = statusCode,
                    Data = dataPointer,
                    ContentType = contentType
                };
            }

            public static HttpResponseDescriptor Alloc(
                HttpStatusCode statusCode,
                byte[] data,
                ContentType contentType)
            {
                ByteArrayDescriptor dataDescriptor = default;
                try
                {
                    dataDescriptor = ByteArrayDescriptor.Alloc(data);

                    return Alloc((int)statusCode, dataDescriptor, (int)contentType);
                }
                catch
                {
                    dataDescriptor.Free();

                    throw;
                }
            }

            public static HttpResponseDescriptor Alloc(HttpResponse response)
                => Alloc(response.StatusCode, response.Data, response.ContentType);

            public HttpResponse ToManaged()
            {
                var statusCode = (HttpStatusCode)descriptor.StatusCode;

                if (!Enum.IsDefined(statusCode))
                    throw new ArgumentOutOfRangeException(nameof(descriptor.StatusCode), descriptor.StatusCode, "Recieved undefined HttpStatusCode");

                var contentType = (ContentType)descriptor.ContentType;

                if (!Enum.IsDefined(contentType))
                    throw new ArgumentOutOfRangeException(nameof(descriptor.ContentType), descriptor.ContentType, "Recieved undefined ContentType");

                var data = descriptor.Data == null ? [] : descriptor.Data->ToManaged();

                return new HttpResponse(statusCode).WithData(data).As(contentType);
            }
        }
    }
}