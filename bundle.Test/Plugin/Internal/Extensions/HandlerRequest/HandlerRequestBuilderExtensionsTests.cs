using Sudzekai.WebOs.Core;
using Sudzekai.WebOs.Core.src.Endpoint.Request.Descriptors;
using Sudzekai.WebOs.Plugin;

namespace bundle.Test.Plugin
{
    public class HandlerRequestBuilderExtensionsTests
    {
        [Fact]
        public unsafe void FromDescriptor_MapsAllRequestFields()
        {
            var (host, hostLength) = Alloc("example.org");
            var (scheme, schemeLength) = Alloc("https");
            var (path, pathLength) = Alloc("/api/items/42");
            var (queryString, queryStringLength) = Alloc("?verbose=true");
            var (contentType, contentTypeLength) = Alloc("application/json");

            var (headerKey, headerKeyLength) = Alloc("Content-Type");
            var headerValues = AllocationUtilities.AllocStringArray(
                ["application/json", "charset=utf-8"],
                out var headerValuesCount);

            var headerDescriptor = new StringArrayEntryDescriptor
            {
                Key = headerKey,
                KeyLength = headerKeyLength,
                Values = headerValues,
                ValuesCount = headerValuesCount
            };

            var headers = AllocationUtilities.CopyArray(new[] { headerDescriptor }, out var headersCount);

            var (claimKey, claimKeyLength) = Alloc("role");
            var claimValues = AllocationUtilities.AllocStringArray(
                ["Administrator", "User"],
                out var claimValuesCount);

            var claimDescriptor = new StringArrayEntryDescriptor
            {
                Key = claimKey,
                KeyLength = claimKeyLength,
                Values = claimValues,
                ValuesCount = claimValuesCount
            };

            var claims = AllocationUtilities.CopyArray(new[] { claimDescriptor }, out var claimsCount);

            var (routeKey, routeKeyLength) = Alloc("id");
            var (routeValue, routeValueLength) = Alloc("42");

            var routeDescriptor = new StringEntryDescriptor
            {
                Key = routeKey,
                KeyLength = routeKeyLength,
                Value = routeValue,
                ValueLength = routeValueLength
            };

            var routeValues = AllocationUtilities.CopyArray(new[] { routeDescriptor }, out var routeValuesCount);

            var (cookieKey, cookieKeyLength) = Alloc("session");
            var (cookieValue, cookieValueLength) = Alloc("abc123");

            var cookieDescriptor = new StringEntryDescriptor
            {
                Key = cookieKey,
                KeyLength = cookieKeyLength,
                Value = cookieValue,
                ValueLength = cookieValueLength
            };

            var cookies = AllocationUtilities.CopyArray(new[] { cookieDescriptor }, out var cookiesCount);

            var body = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF };
            var bodyPointer = AllocationUtilities.CopyToUnmanaged(body, out var bodyLength);

            var descriptor = new HandlerRequestDescriptor
            {
                HttpMethod = (int)HandlerHttpMethod.Delete,

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

                Body = bodyPointer,
                BodyLength = bodyLength
            };

            var request = HandlerRequest.FromDescriptor(descriptor);

            Assert.Equal(HandlerHttpMethod.Delete, request.HttpMethod);
            Assert.Equal("example.org", request.Host);
            Assert.Equal("https", request.Scheme);
            Assert.Equal("/api/items/42", request.Path);
            Assert.Equal("?verbose=true", request.QueryString);
            Assert.Equal("application/json", request.ContentType);
            Assert.Equal(body, request.Body);

            var header = Assert.Single(request.Headers);
            Assert.Equal("Content-Type", header.Key);
            Assert.Equal(new[] { "application/json", "charset=utf-8" }, header.Value);

            var routeValueEntry = Assert.Single(request.RouteValues);
            Assert.Equal("id", routeValueEntry.Key);
            Assert.Equal("42", routeValueEntry.Value);

            var cookie = Assert.Single(request.Cookies);
            Assert.Equal("session", cookie.Key);
            Assert.Equal("abc123", cookie.Value);

            var claim = Assert.Single(request.Claims);
            Assert.Equal("role", claim.Key);
            Assert.Equal(new[] { "Administrator", "User" }, claim.Value);
        }

        [Fact]
        public void FromDescriptor_MapsEmptyRequestToEmptyValues()
        {
            var descriptor = new HandlerRequestDescriptor
            {
                HttpMethod = (int)HandlerHttpMethod.Get
            };

            var request = HandlerRequest.FromDescriptor(descriptor);

            Assert.Equal(HandlerHttpMethod.Get, request.HttpMethod);
            Assert.Equal(string.Empty, request.Host);
            Assert.Equal(string.Empty, request.Scheme);
            Assert.Equal(string.Empty, request.Path);
            Assert.Equal(string.Empty, request.QueryString);
            Assert.Equal(string.Empty, request.ContentType);
            Assert.Empty(request.Headers);
            Assert.Empty(request.RouteValues);
            Assert.Empty(request.Cookies);
            Assert.Empty(request.Claims);
            Assert.Empty(request.Body!);
        }

        private static (nint Pointer, int Length) Alloc(string value)
        {
            var pointer = AllocationUtilities.AllocString(value, out var length);

            return (pointer, length);
        }
    }
}
