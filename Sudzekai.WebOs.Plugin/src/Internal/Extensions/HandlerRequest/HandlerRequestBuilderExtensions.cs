using Sudzekai.WebOs.Core;
using Sudzekai.WebOs.Core.src.Endpoint.Request.Descriptors;

namespace Sudzekai.WebOs.Plugin
{
    internal static class HandlerRequestBuilderExtensions
    {
        extension (HandlerRequest)
        {
            public static unsafe HandlerRequest FromDescriptor(HandlerRequestDescriptor descriptor)
            {
                var httpMethod = (HandlerHttpMethod)descriptor.HttpMethod;

                if (!Enum.IsDefined(httpMethod))
                    throw new InvalidOperationException("дескриптор содержал невалидный http method");

                var headers = AllocationUtilities.ToArray(descriptor.Headers, descriptor.HeadersCount);
                var routeValues = AllocationUtilities.ToArray(descriptor.RouteValues, descriptor.RouteValuesCount);
                var cookies = AllocationUtilities.ToArray(descriptor.Cookies, descriptor.CookiesCount);
                var claims = AllocationUtilities.ToArray(descriptor.Claims, descriptor.ClaimsCount);

                HandlerRequest request = new()
                {
                    HttpMethod = httpMethod,
                    
                    Host = AllocationUtilities.ToString(descriptor.Host, descriptor.HostLength),
                    Scheme = AllocationUtilities.ToString(descriptor.Scheme, descriptor.SchemeLength),
                    Path = AllocationUtilities.ToString(descriptor.Path, descriptor.PathLength),
                    
                    QueryString = AllocationUtilities.ToString(descriptor.QueryString, descriptor.QueryStringLength),
                    
                    Headers = headers.Select(ArrPairFromDescriptor).ToDictionary(),
                    RouteValues = routeValues.Select(PairFromDescriptor).ToDictionary(),
                    Cookies = cookies.Select(PairFromDescriptor).ToDictionary(),
                    Claims = claims.Select(ArrPairFromDescriptor).ToDictionary(),
                    
                    ContentType = AllocationUtilities.ToString(descriptor.ContentType, descriptor.ContentTypeLength),
                    Body = AllocationUtilities.ToBytes(descriptor.Body, descriptor.BodyLength),
                };

                descriptor.Dispose();

                return request;
            }
        }

        private static unsafe KeyValuePair<string, string[]> ArrPairFromDescriptor(StringArrayEntryDescriptor descriptor)
        {
            var key = AllocationUtilities.ToString(descriptor.Key, descriptor.KeyLength);
            var values = AllocationUtilities.ToStringArray(descriptor.Values, descriptor.ValuesCount);
            return new(key,values);

        }

        private static KeyValuePair<string, string> PairFromDescriptor(StringEntryDescriptor descriptor)
        {
            var key = AllocationUtilities.ToString(descriptor.Key, descriptor.KeyLength);
            var value = AllocationUtilities.ToString(descriptor.Value, descriptor.ValueLength);
            return new(key, value);
        }
    }
}
