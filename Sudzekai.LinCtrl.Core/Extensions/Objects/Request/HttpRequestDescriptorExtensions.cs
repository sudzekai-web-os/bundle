
namespace Sudzekai.LinCtrl.Core 
{
    public static unsafe class HttpRequestDescriptorExtensions
    {
        extension(HttpRequestDescriptor descriptor)
        {
            public static HttpRequestDescriptor Alloc(
                int httpMethod,
                StringDescriptor host,
                StringDescriptor scheme,
                StringDescriptor path,
                StringDescriptor queryString,
                StringToStringArrayDictionaryDescriptor headers,
                StringToStringArrayDictionaryDescriptor claims,
                StringToStringDictionaryDescriptor routeValues,
                StringToStringDictionaryDescriptor cookies,
                int contentType,
                ByteArrayDescriptor body)
            {
                StringDescriptor* hostPointer = null;
                StringDescriptor* schemePointer = null;
                StringDescriptor* pathPointer = null;
                StringDescriptor* queryStringPointer = null;

                StringToStringArrayDictionaryDescriptor* headersDictionaryPointer = null;
                StringToStringArrayDictionaryDescriptor* claimsDictionaryPointer = null;

                StringToStringDictionaryDescriptor* routeValuesDictionaryPointer = null;
                StringToStringDictionaryDescriptor* cookiesDictionaryPointer = null;

                ByteArrayDescriptor* bodyArrayPointer = null;

                try
                {
                    hostPointer = AllocationUtilities.AllocCoTaskMem(host);
                    schemePointer = AllocationUtilities.AllocCoTaskMem(scheme);
                    pathPointer = AllocationUtilities.AllocCoTaskMem(path);
                    queryStringPointer = AllocationUtilities.AllocCoTaskMem(queryString);

                    headersDictionaryPointer = AllocationUtilities.AllocCoTaskMem(headers);
                    claimsDictionaryPointer = AllocationUtilities.AllocCoTaskMem(claims);

                    routeValuesDictionaryPointer = AllocationUtilities.AllocCoTaskMem(routeValues);
                    cookiesDictionaryPointer = AllocationUtilities.AllocCoTaskMem(cookies);

                    bodyArrayPointer = AllocationUtilities.AllocCoTaskMem(body);
                }
                catch
                {
                    AllocationUtilities.FreeCoTaskMemIfNotNull(ref hostPointer);
                    AllocationUtilities.FreeCoTaskMemIfNotNull(ref schemePointer);
                    AllocationUtilities.FreeCoTaskMemIfNotNull(ref pathPointer);
                    AllocationUtilities.FreeCoTaskMemIfNotNull(ref queryStringPointer);

                    AllocationUtilities.FreeCoTaskMemIfNotNull(ref headersDictionaryPointer);
                    AllocationUtilities.FreeCoTaskMemIfNotNull(ref claimsDictionaryPointer);

                    AllocationUtilities.FreeCoTaskMemIfNotNull(ref routeValuesDictionaryPointer);
                    AllocationUtilities.FreeCoTaskMemIfNotNull(ref cookiesDictionaryPointer);

                    AllocationUtilities.FreeCoTaskMemIfNotNull(ref bodyArrayPointer);

                    throw;
                }

                return new()
                {
                    HttpMethod = httpMethod,
                    Host = hostPointer,
                    Scheme = schemePointer,
                    Path = pathPointer,
                    QueryString = queryStringPointer,
                    Headers = headersDictionaryPointer,
                    Claims = claimsDictionaryPointer,
                    RouteValues = routeValuesDictionaryPointer,
                    Cookies = cookiesDictionaryPointer,
                    ContentType = contentType,
                    Body = bodyArrayPointer
                };
            }

            public static HttpRequestDescriptor Alloc(
                RequestHandlerHttpMethod httpMethod,
                string host,
                string scheme,
                string path,
                string queryString,
                Dictionary<string, string[]> headers,
                Dictionary<string, string[]> claims,
                Dictionary<string, string> routeValues,
                Dictionary<string, string> cookies,
                ContentType contentType,
                byte[] body)
            {
                StringDescriptor hostDescriptor = default;
                StringDescriptor schemeDescriptor = default;
                StringDescriptor pathDescriptor = default;
                StringDescriptor queryStringDescriptor = default;

                StringToStringArrayDictionaryDescriptor headersDictionaryDescriptor = default;
                StringToStringArrayDictionaryDescriptor claimsDictionaryDescriptor = default;

                StringToStringDictionaryDescriptor routeValuesDictionaryDescriptor = default;
                StringToStringDictionaryDescriptor cookiesDictionaryDescriptor = default;

                ByteArrayDescriptor bodyArrayDescriptor = default;

                try
                {
                    hostDescriptor = StringDescriptor.Alloc(host);
                    schemeDescriptor = StringDescriptor.Alloc(scheme);
                    pathDescriptor = StringDescriptor.Alloc(path);
                    queryStringDescriptor = StringDescriptor.Alloc(queryString);

                    headersDictionaryDescriptor = StringToStringArrayDictionaryDescriptor.Alloc(headers);
                    claimsDictionaryDescriptor = StringToStringArrayDictionaryDescriptor.Alloc(claims);

                    routeValuesDictionaryDescriptor = StringToStringDictionaryDescriptor.Alloc(routeValues);
                    cookiesDictionaryDescriptor = StringToStringDictionaryDescriptor.Alloc(cookies);

                    bodyArrayDescriptor = ByteArrayDescriptor.Alloc(body);

                    return Alloc(
                        (int)httpMethod,
                        hostDescriptor,
                        schemeDescriptor,
                        pathDescriptor,
                        queryStringDescriptor,
                        headersDictionaryDescriptor,
                        claimsDictionaryDescriptor,
                        routeValuesDictionaryDescriptor,
                        cookiesDictionaryDescriptor,
                        (int)contentType,
                        bodyArrayDescriptor);
                }
                catch
                {
                    hostDescriptor.Free();
                    schemeDescriptor.Free();
                    pathDescriptor.Free();
                    queryStringDescriptor.Free();

                    headersDictionaryDescriptor.Free();
                    claimsDictionaryDescriptor.Free();

                    routeValuesDictionaryDescriptor.Free();
                    cookiesDictionaryDescriptor.Free();

                    bodyArrayDescriptor.Free();

                    throw;
                }
            }

            public static HttpRequestDescriptor Alloc(HttpRequestDto request)
                => Alloc(
                    request.HttpMethod,
                    request.Host,
                    request.Scheme,
                    request.Path,
                    request.QueryString,
                    request.Headers,
                    request.Claims,
                    request.RouteValues,
                    request.Cookies,
                    request.ContentType,
                    request.Body);

            public HttpRequestDto ToManaged()
            {
                var httpMethod = (RequestHandlerHttpMethod)descriptor.HttpMethod;

                if (!Enum.IsDefined(httpMethod))
                    throw new ArgumentOutOfRangeException(nameof(descriptor.HttpMethod), descriptor.HttpMethod, "Received undefined HttpMethod");

                var host = descriptor.Host is null
                    ? string.Empty : descriptor.Host->ToManaged();

                var scheme = descriptor.Scheme is null
                    ? string.Empty : descriptor.Scheme->ToManaged();

                var path = descriptor.Path is null
                    ? string.Empty : descriptor.Path->ToManaged();

                var queryString = descriptor.QueryString is null
                    ? string.Empty : descriptor.QueryString->ToManaged();

                var headers = descriptor.Headers is null
                    ? [] : descriptor.Headers->ToManaged();

                var claims = descriptor.Claims is null
                    ? [] : descriptor.Claims->ToManaged();

                var routeValues = descriptor.RouteValues is null
                    ? [] : descriptor.RouteValues->ToManaged();

                var cookies = descriptor.Cookies is null
                    ? [] : descriptor.Cookies->ToManaged();

                var contentType = (ContentType)descriptor.ContentType;

                if (!Enum.IsDefined(contentType))
                    throw new ArgumentOutOfRangeException(nameof(descriptor.ContentType), descriptor.ContentType, "Received undefined ContentType");

                var body = descriptor.Body is null
                    ? [] : descriptor.Body->ToManaged();

                return new()
                {
                    HttpMethod = httpMethod,
                    Host = host,
                    Scheme = scheme,
                    Path = path,
                    QueryString = queryString,
                    Headers = headers,
                    Claims = claims,
                    RouteValues = routeValues,
                    Cookies = cookies,
                    ContentType = contentType,
                    Body = body,
                };
            }
        }
    }
}