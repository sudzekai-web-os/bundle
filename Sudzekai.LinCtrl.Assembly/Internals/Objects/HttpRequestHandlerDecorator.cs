using Sudzekai.LinCtrl.Abi.Managed;
using Microsoft.Extensions.Logging;
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Assembly.Internals.Objects
{
    internal class HttpRequestHandlerDecorator(
        IRequestHandler handler,
        ILogger logger) : IRequestHandler
    {
        public string Pattern => handler.Pattern;

        public HttpRequestMethod HttpMethod => handler.HttpMethod;

        public bool IsFiltered => handler.IsFiltered;

        public HandlerMetadata? Metadata => handler.Metadata;

        public HttpResponseDto Handle(HttpRequestDto request)
        {
            try
            {
                return handler.Handle(request);
            }
            catch (Exception ex)
            {
                logger.LogError("[{method}:{path}] Exception occured while handling http request: {message}", HttpMethod.ToString().ToLower(), Pattern, ex.Message);
                return new()
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    ContentType = ResponseContentType.TextPlain,
                    Data = "Internal server error"u8.ToArray()
                };
            }
        }
    }
}
