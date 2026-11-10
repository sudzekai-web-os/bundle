using Sudzekai.LinCtrl.Abi.Allocation.Types.Exceptions;

namespace Sudzekai.LinCtrl.Abi.Allocation
{
    public static class HttpRequestDtoAllocator
    {
        public static HttpRequestDtoDescriptor Alloc(
            int httpMethod,
            StringViewer host,
            StringViewer scheme,
            StringViewer path,
            StringViewer queryString,
            StringToStringArrayDictionaryDescriptor headers,
            StringToStringArrayDictionaryDescriptor claims,
            StringToStringDictionaryDescriptor routeValues,
            StringToStringDictionaryDescriptor cookies,
            int contentType,
            ByteArrayViewer body)
        {
            ArgumentNotAllocatedException.ThrowIfNotAllocated(host, nameof(host));
            ArgumentNotAllocatedException.ThrowIfNotAllocated(host, nameof(scheme));
            ArgumentNotAllocatedException.ThrowIfNotAllocated(host, nameof(path));
            ArgumentNotAllocatedException.ThrowIfNotAllocated(host, nameof(queryString));
            ArgumentNotAllocatedException.ThrowIfNotAllocated(host, nameof(headers));
            ArgumentNotAllocatedException.ThrowIfNotAllocated(host, nameof(claims));
            ArgumentNotAllocatedException.ThrowIfNotAllocated(host, nameof(routeValues));
            ArgumentNotAllocatedException.ThrowIfNotAllocated(host, nameof(cookies));
            ArgumentNotAllocatedException.ThrowIfNotAllocated(host, nameof(body));

            return new(
                httpMethod,
                host,
                scheme,
                path,
                queryString,
                headers,
                claims,
                routeValues,
                cookies,
                contentType,
                body);
        }

        public static HttpRequestDtoDescriptor Alloc(
            int httpMethod,
            string host,
            string scheme,
            string path,
            string queryString,
            Dictionary<string, string[]> headers,
            Dictionary<string, string[]> claims,
            Dictionary<string, string> routeValues,
            Dictionary<string, string> cookies,
            int contentType,
            byte[] body)
        {
            ArgumentNullException.ThrowIfNull(host);
            ArgumentNullException.ThrowIfNull(scheme);
            ArgumentNullException.ThrowIfNull(path);
            ArgumentNullException.ThrowIfNull(queryString);

            StringViewer hostViewer = default;
            StringViewer schemeViewer = default;
            StringViewer pathViewer = default;
            StringViewer queryStringViewer = default;

            StringToStringArrayDictionaryDescriptor headersDescriptor = default;
            StringToStringArrayDictionaryDescriptor claimsDescriptor = default;

            StringToStringDictionaryDescriptor routeValuesDescriptor = default;
            StringToStringDictionaryDescriptor cookiesDescriptor = default;

            ByteArrayViewer bodyDescriptor = default;

            try
            {
                hostViewer = StringAllocator.Alloc(host);
                schemeViewer = StringAllocator.Alloc(scheme);
                pathViewer = StringAllocator.Alloc(path);
                queryStringViewer = StringAllocator.Alloc(queryString);

                headersDescriptor = StringToStringArrayDictionaryAllocator.Alloc(headers);
                claimsDescriptor = StringToStringArrayDictionaryAllocator.Alloc(claims);

                routeValuesDescriptor = StringToStringDictionaryAllocator.Alloc(routeValues);
                cookiesDescriptor = StringToStringDictionaryAllocator.Alloc(cookies);

                bodyDescriptor = ByteArrayAllocator.Alloc(body);

                return Alloc(httpMethod,
                    hostViewer,
                    schemeViewer,
                    pathViewer,
                    queryStringViewer,
                    headersDescriptor,
                    claimsDescriptor,
                    routeValuesDescriptor,
                    cookiesDescriptor,
                    contentType,
                    bodyDescriptor);
            }
            catch
            {
                StringAllocator.Free(ref hostViewer);
                StringAllocator.Free(ref schemeViewer);
                StringAllocator.Free(ref pathViewer);
                StringAllocator.Free(ref queryStringViewer);

                StringToStringArrayDictionaryAllocator.Free(ref headersDescriptor);
                StringToStringArrayDictionaryAllocator.Free(ref claimsDescriptor);

                StringToStringDictionaryAllocator.Free(ref routeValuesDescriptor);
                StringToStringDictionaryAllocator.Free(ref cookiesDescriptor);

                ByteArrayAllocator.Free(ref bodyDescriptor);

                throw;
            }
        }

        public static void Free(ref HttpRequestDtoDescriptor descriptor)
        {
            if (!descriptor.IsAllocated)
                return;

            StringAllocator.Free(ref descriptor.Host);
            StringAllocator.Free(ref descriptor.Scheme);
            StringAllocator.Free(ref descriptor.Path);
            StringAllocator.Free(ref descriptor.QueryString);

            StringToStringArrayDictionaryAllocator.Free(ref descriptor.Headers);
            StringToStringArrayDictionaryAllocator.Free(ref descriptor.Claims);

            StringToStringDictionaryAllocator.Free(ref descriptor.RouteValues);
            StringToStringDictionaryAllocator.Free(ref descriptor.Cookies);

            ByteArrayAllocator.Free(ref descriptor.Body);
        }
    }
}
