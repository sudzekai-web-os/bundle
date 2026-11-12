using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Extensions
{
    public static class RequestHandlerOpenApiExtensions
    {
        extension(IEnumerable<IRequestHandler> handlers)
        {
            public string WriteOpenApiMetadata()
                => OpenApiMetadataWriter.Write(
                    handlers.Select(handler => (handler.Pattern, handler.HttpMethod, handler.Metadata, handler.IsFiltered)),
                    type => Enum.IsDefined(type) && type != ResponseContentType.None
                        ? type.ToMimeString()
                        : ResponseContentType.ApplicationOctetStream.ToMimeString());
        }
    }
}
