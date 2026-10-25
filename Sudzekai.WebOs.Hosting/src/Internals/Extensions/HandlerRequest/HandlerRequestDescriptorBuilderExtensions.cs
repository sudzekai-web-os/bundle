using Sudzekai.WebOs.Core;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Sudzekai.WebOs.Hosting
{
    internal static unsafe class HandlerRequestDescriptorBuilderExtensions
    {
        public static HandlerRequestDescriptor ToDescriptor(this HandlerRequest request)
        {
            var path = AllocationUtilities.AllocString(request.Path, out var pathLength);

            var host = AllocationUtilities.AllocString(request.Host, out var hostLength);

            var scheme = AllocationUtilities.AllocString(request.Scheme, out var schemeLength);

            var queryString = AllocationUtilities.AllocString(request.QueryString, out var queryStringLength);

            var contentType = AllocationUtilities.AllocString(request.ContentType, out var contentTypeLength);

            var body = AllocationUtilities.CopyToUnmanaged(request.Body, out var bodyLength);

            var headers = AllocationUtilities.CopyArray(request.Headers.Select(ToDescriptor).ToArray(), out var headersCount);

            var routeValues = AllocationUtilities.CopyArray(request.RouteValues.Select(ToDescriptor).ToArray(), out var routeValuesCount);

            var cookies = AllocationUtilities.CopyArray(request.Cookies.Select(ToDescriptor).ToArray(), out var cookiesCount);

            var claims = AllocationUtilities.CopyArray(request.Claims.Select(ToDescriptor).ToArray(), out var claimsCount);

            return new HandlerRequestDescriptor
            {
                HttpMethod = (int)request.HttpMethod,

                Host = host,
                HostLength = hostLength,

                Scheme = scheme,
                SchemeLength = schemeLength,

                Path = path,
                PathLength = pathLength,

                QueryString = queryString,
                QueryStringLength = queryStringLength,

                Headers = headers,
                HeadersCount = headersCount,

                RouteValues = routeValues,
                RouteValuesCount = routeValuesCount,

                Cookies = cookies,
                CookiesCount = cookiesCount,

                Claims = claims,
                ClaimsCount = claimsCount,

                ContentType = contentType,
                ContentTypeLength = contentTypeLength,

                Body = body,
                BodyLength = bodyLength,
            };
        }

        private static StringArrayEntryDescriptor ToDescriptor(this KeyValuePair<string, string[]> headerEntry)
        {
            var key = AllocationUtilities.AllocString(headerEntry.Key, out var keyLength);

            var values = AllocationUtilities.AllocStringArray(headerEntry.Value, out var valuesCount);

            return new StringArrayEntryDescriptor
            {
                Key = key,
                KeyLength = keyLength,
                Values = values,
                ValuesCount = valuesCount
            };
        }

        private static StringEntryDescriptor ToDescriptor(this KeyValuePair<string, string> routeEntry)
        {
            var key = AllocationUtilities.AllocString(routeEntry.Key, out var keyLength);

            var valueObject = Convert.ToString(routeEntry.Value, CultureInfo.InvariantCulture) ?? string.Empty;

            var value = AllocationUtilities.AllocString(valueObject, out var valueLength);

            return new StringEntryDescriptor
            {
                Key = key,
                KeyLength = keyLength,
                Value = value,
                ValueLength = valueLength
            };
        }
    }
}
