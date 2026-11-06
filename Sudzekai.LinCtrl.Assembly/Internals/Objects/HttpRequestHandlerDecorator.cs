using Microsoft.Extensions.Logging;
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Assembly.Internals.Objects
{
    internal class HttpRequestHandlerDecorator(
        IHttpRequestHandler handler, 
        ILogger logger) : IHttpRequestHandler
    {
        public string Pattern => handler.Pattern;

        public RequestHandlerHttpMethod HttpMethod => handler.HttpMethod;

        public async Task<HttpResponse> HandleAsync(HttpRequestDto request)
        {
            try
            {
                return await handler.HandleAsync(request);
            }
            catch (Exception ex)
            {
                logger.LogError("[{method}:{path}] Exception occured while handling http request: {message}", HttpMethod.ToString().ToLower(), Pattern, ex.Message);
                return HttpResponse.Create(System.Net.HttpStatusCode.InternalServerError).As(ContentType.ApplicationJson).WithData("Internal server error");
            }
        }
    }
}
