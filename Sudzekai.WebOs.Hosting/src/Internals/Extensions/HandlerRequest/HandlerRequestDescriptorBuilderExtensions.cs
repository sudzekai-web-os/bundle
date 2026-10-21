using Sudzekai.WebOs.Core;

namespace Sudzekai.WebOs.Hosting
{ 
    internal static unsafe class HandlerRequestDescriptorBuilderExtensions
    {
        public static HandlerRequestDescriptor ToDescriptor(this HandlerRequest request)
        {
            var path = AllocationUtilities.AllocString(request.Path, out var pathLength);

            var queryString = AllocationUtilities.AllocString(request.QueryString, out var queryStringLength);

            var body = AllocationUtilities.CopyToUnmanaged(request.Body, out var bodyLength);

            var headers = AllocationUtilities.CopyArray(request.Headers.Select(ToDescriptor).ToArray(), out var headersCount);

            return new HandlerRequestDescriptor
            {
                Path = path,
                PathLength = pathLength,
                HttpMethod = (int)request.HttpMethod,
                QueryString = queryString,
                QueryStringLength = queryStringLength,
                Body = body,
                BodyLength = bodyLength,
                Headers = headers,
                HeadersCount = headersCount
            };
        }

        private static HeaderEntryDescriptor ToDescriptor(this HeaderEntry headerEntry)
        {
            var key = AllocationUtilities.AllocString(headerEntry.Key, out var keyLength);

            var values = AllocationUtilities.AllocStringArray(headerEntry.Values, out var valuesCount);

            return new HeaderEntryDescriptor
            {
                Key = key,
                KeyLength = keyLength,
                Values = values,
                ValuesCount = valuesCount
            };
        }
    }
}
