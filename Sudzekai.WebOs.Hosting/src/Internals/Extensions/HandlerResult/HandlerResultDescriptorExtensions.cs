using Sudzekai.WebOs.Core;
using System.Net;
using System.Runtime.InteropServices;

namespace Sudzekai.WebOs.Hosting
{
    internal static class HandlerResultDescriptorExtensions
    {
        extension(HandlerResult)
        {
            public static HandlerResult FromDescriptor(HandlerResultDescriptor descriptor)
            {
                var statusCode = (HttpStatusCode)descriptor.StatusCode;

                if (!Enum.IsDefined(statusCode))
                    throw new InvalidOperationException("дескриптор содержал невалидный http status code");

                var result = HandlerResult.Create(statusCode);

                var contentType = (ContentType)descriptor.ContentType;

                if (!Enum.IsDefined(contentType))
                    throw new InvalidOperationException($"Неизвестный тип данных дескриптора: {descriptor.ContentType}");

                result.As(contentType);

                result.WithData(AllocationUtilities.ToString(descriptor.Data, descriptor.DataLength));

                Marshal.FreeCoTaskMem(descriptor.Data);

                return result;
            }
        }
    }
}
