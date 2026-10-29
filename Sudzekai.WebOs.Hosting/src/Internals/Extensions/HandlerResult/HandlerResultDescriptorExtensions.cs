using Sudzekai.WebOs.Core;
using System.Net;
using System.Runtime.InteropServices;

namespace Sudzekai.WebOs.Hosting
{
    internal static class HandlerResultDescriptorExtensions
    {
        extension(HandlerResult)
        {
            public static HandlerResult FromDescriptor(ref HandlerResultDescriptor descriptor)
            {
                var statusCode = (HttpStatusCode)descriptor.StatusCode;

                if (!Enum.IsDefined(statusCode))
                    throw new InvalidOperationException(LogsDictionary.HandlerResultDescriptorExtensions.InvalidStatusCode);

                var result = HandlerResult.Create(statusCode);

                var contentType = (ContentType)descriptor.ContentType;

                if (!Enum.IsDefined(contentType))
                    throw new InvalidOperationException(string.Format(LogsDictionary.HandlerResultDescriptorExtensions.UnknownContentType, descriptor.ContentType.ToString()));

                result.As(contentType);

                result.WithData(AllocationUtilities.ToString(descriptor.Data, descriptor.DataLength));

                descriptor.Dispose();

                return result;
            }
        }
    }
}
