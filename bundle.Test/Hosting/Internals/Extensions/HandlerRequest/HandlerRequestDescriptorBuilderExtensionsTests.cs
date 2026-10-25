using Sudzekai.WebOs.Core;
using Sudzekai.WebOs.Hosting;

namespace bundle.Test.Hosting
{
    public class HandlerRequestDescriptorBuilderExtensionsTests
    {
        [Fact]
        public unsafe void ToDescriptor_MapsAllRequestFields()
        {
            var request = new HandlerRequest
            {
                HttpMethod = HandlerHttpMethod.Put,

                Host = "example.org",
                Scheme = "https",
                Path = "/api/items/42",

                QueryString = "?verbose=true",

                Headers = new Dictionary<string, string[]>
                {
                    ["Content-Type"] = ["application/json", "charset=utf-8"]
                },

                RouteValues = new Dictionary<string, string>
                {
                    ["id"] = "42"
                },

                Cookies = new Dictionary<string, string>
                {
                    ["session"] = "abc123"
                },

                Claims = new Dictionary<string, string[]>
                {
                    ["role"] = ["Administrator", "User"]
                },

                ContentType = "application/json",
                Body = [0x01, 0x02, 0x03]
            };

            var descriptor = request.ToDescriptor();

            try
            {
                Assert.Equal((int)HandlerHttpMethod.Put, descriptor.HttpMethod);

                Assert.Equal(
                    "example.org",
                    AllocationUtilities.ToString(descriptor.Host, descriptor.HostLength));
                Assert.Equal(
                    "https",
                    AllocationUtilities.ToString(descriptor.Scheme, descriptor.SchemeLength));
                Assert.Equal(
                    "/api/items/42",
                    AllocationUtilities.ToString(descriptor.Path, descriptor.PathLength));
                Assert.Equal(
                    "?verbose=true",
                    AllocationUtilities.ToString(descriptor.QueryString, descriptor.QueryStringLength));
                Assert.Equal(
                    "application/json",
                    AllocationUtilities.ToString(descriptor.ContentType, descriptor.ContentTypeLength));
                Assert.Equal(
                    new byte[] { 0x01, 0x02, 0x03 },
                    AllocationUtilities.ToBytes(descriptor.Body, descriptor.BodyLength));

                var headers = AllocationUtilities.ToArray(descriptor.Headers, descriptor.HeadersCount);
                var header = Assert.Single(headers);

                Assert.Equal(
                    "Content-Type",
                    AllocationUtilities.ToString(header.Key, header.KeyLength));
                Assert.Equal(
                    new[] { "application/json", "charset=utf-8" },
                    AllocationUtilities.ToStringArray(header.Values, header.ValuesCount));

                var routeValues = AllocationUtilities.ToArray(descriptor.RouteValues, descriptor.RouteValuesCount);
                var routeValue = Assert.Single(routeValues);

                Assert.Equal(
                    "id",
                    AllocationUtilities.ToString(routeValue.Key, routeValue.KeyLength));
                Assert.Equal(
                    "42",
                    AllocationUtilities.ToString(routeValue.Value, routeValue.ValueLength));

                var cookies = AllocationUtilities.ToArray(descriptor.Cookies, descriptor.CookiesCount);
                var cookie = Assert.Single(cookies);

                Assert.Equal(
                    "session",
                    AllocationUtilities.ToString(cookie.Key, cookie.KeyLength));
                Assert.Equal(
                    "abc123",
                    AllocationUtilities.ToString(cookie.Value, cookie.ValueLength));

                var claims = AllocationUtilities.ToArray(descriptor.Claims, descriptor.ClaimsCount);
                var claim = Assert.Single(claims);

                Assert.Equal(
                    "role",
                    AllocationUtilities.ToString(claim.Key, claim.KeyLength));
                Assert.Equal(
                    new[] { "Administrator", "User" },
                    AllocationUtilities.ToStringArray(claim.Values, claim.ValuesCount));
            }
            finally
            {
                descriptor.Dispose();
            }
        }

        [Fact]
        public unsafe void ToDescriptor_MapsEmptyValuesToZeroLengths()
        {
            var request = new HandlerRequest
            {
                HttpMethod = HandlerHttpMethod.Get,
                Host = null,
                Scheme = string.Empty,
                Path = string.Empty,
                QueryString = string.Empty,
                ContentType = null,
                Body = []
            };

            var descriptor = request.ToDescriptor();

            try
            {
                Assert.Equal((int)HandlerHttpMethod.Get, descriptor.HttpMethod);

                Assert.Equal(nint.Zero, descriptor.Host);
                Assert.Equal(0, descriptor.HostLength);

                Assert.Equal(nint.Zero, descriptor.Scheme);
                Assert.Equal(0, descriptor.SchemeLength);

                Assert.Equal(nint.Zero, descriptor.Path);
                Assert.Equal(0, descriptor.PathLength);

                Assert.Equal(nint.Zero, descriptor.QueryString);
                Assert.Equal(0, descriptor.QueryStringLength);

                Assert.Equal(nint.Zero, descriptor.ContentType);
                Assert.Equal(0, descriptor.ContentTypeLength);

                Assert.Equal(nint.Zero, descriptor.Body);
                Assert.Equal(0, descriptor.BodyLength);

                Assert.True(descriptor.Headers == null);
                Assert.Equal(0, descriptor.HeadersCount);

                Assert.True(descriptor.RouteValues == null);
                Assert.Equal(0, descriptor.RouteValuesCount);

                Assert.True(descriptor.Cookies == null);
                Assert.Equal(0, descriptor.CookiesCount);

                Assert.True(descriptor.Claims == null);
                Assert.Equal(0, descriptor.ClaimsCount);
            }
            finally
            {
                descriptor.Dispose();
            }
        }
    }
}
