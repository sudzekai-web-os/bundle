using Sudzekai.WebOs.Core;

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

                HandlerRequest request = new()
                {
                    HttpMethod = httpMethod,
                    Path = AllocationUtilities.ToString(descriptor.Path, descriptor.PathLength),
                    QueryString = AllocationUtilities.ToString(descriptor.QueryString, descriptor.QueryStringLength),
                    Headers = [.. headers.Select(HeaderEntry.FromDescriptor)],
                    Body = AllocationUtilities.ToBytes(descriptor.Body, descriptor.BodyLength),
                };

                // Освобождение всей памяти (Path/QueryString/Body + заголовки) происходит здесь,
                // один раз через HandlerRequestDescriptor.Dispose() (FreeHeaders освобождает каждый header).
                descriptor.Dispose();

                return request;
            }
        }

        extension (HeaderEntry)
        {
            public static unsafe HeaderEntry FromDescriptor(HeaderEntryDescriptor descriptor)
            {
                var key = AllocationUtilities.ToString(descriptor.Key, descriptor.KeyLength);
                var values = AllocationUtilities.ToStringArray(descriptor.Values, descriptor.ValuesCount);

                // НЕ освобождаем Key/Values здесь — они освобождаются родительским
                // HandlerRequestDescriptor.Dispose() (FreeHeaders → (&headers[i])->Dispose()).
                // Иначе те же указатели освобождаются дважды → double-free → краш.

                return new HeaderEntry
                {
                    Key = key,
                    Values = values
                };
            }
        }
    }
}
