using Sudzekai.LinCtrl.Abi.Allocation;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Core;
using System.Net;

namespace Sudzekai.LinCtrl.Abi.Mapping
{
    public static unsafe class HttpDtoMapper
    {
        public static HttpRequestDtoDescriptor ToDescriptor(HttpRequestDto request)
            => HttpRequestDtoAllocator.Alloc((int)request.HttpMethod, request.Host, request.Scheme,
                request.Path, request.QueryString, request.Headers, request.Claims, request.RouteValues,
                request.Cookies, (int)request.ContentType, request.Body);

        public static HttpResponseDtoDescriptor ToDescriptor(HttpResponseDto response)
            => HttpResponseDtoAllocator.Alloc((int)response.ContentType, (int)response.StatusCode, response.Data);

        public static HttpResponseDto ToManaged(HttpResponseDtoDescriptor response)
            => new()
            {
                ContentType = (ResponseContentType)response.ContentType,
                StatusCode = (HttpStatusCode)response.StatusCode,
                Data = ToManaged(response.Data)
            };

        public static HttpRequestDto ToManaged(HttpRequestDtoDescriptor request)
        {
            var result = new HttpRequestDto
            {
                HttpMethod = (HttpRequestMethod)request.HttpMethod,
                Host = StringMapper.ToManaged(request.Host),
                Scheme = StringMapper.ToManaged(request.Scheme),
                Path = StringMapper.ToManaged(request.Path),
                QueryString = StringMapper.ToManaged(request.QueryString),
                ContentType = (ResponseContentType)request.ContentType,
                Body = ToManaged(request.Body)
            };

            for (int i = 0; i < request.Headers.Count; i++)
                result.Headers.Add(StringMapper.ToManaged(request.Headers.Array[i].Key), ToManaged(request.Headers.Array[i].Value));
            for (int i = 0; i < request.Claims.Count; i++)
                result.Claims.Add(StringMapper.ToManaged(request.Claims.Array[i].Key), ToManaged(request.Claims.Array[i].Value));
            for (int i = 0; i < request.RouteValues.Count; i++)
                result.RouteValues.Add(StringMapper.ToManaged(request.RouteValues.Array[i].Key), StringMapper.ToManaged(request.RouteValues.Array[i].Value));
            for (int i = 0; i < request.Cookies.Count; i++)
                result.Cookies.Add(StringMapper.ToManaged(request.Cookies.Array[i].Key), StringMapper.ToManaged(request.Cookies.Array[i].Value));

            return result;
        }

        private static byte[] ToManaged(ByteArrayViewer viewer)
            => new ReadOnlySpan<byte>(viewer.Array, viewer.Count).ToArray();

        private static string[] ToManaged(StringArrayDescriptor descriptor)
        {
            var result = new string[descriptor.Count];
            for (int i = 0; i < descriptor.Count; i++)
                result[i] = StringMapper.ToManaged(descriptor.Array[i]);
            return result;
        }
    }
}
