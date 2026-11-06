using LinCtrlAssembly.Objects;
using Sudzekai.LinCtrl.Core;
using System.Net;

namespace LinCtrlAssembly.Handlers
{
    internal class ResponseObjectRequestHandler : IHttpRequestHandler
    {
        public string Pattern => "object";

        public RequestHandlerHttpMethod HttpMethod => RequestHandlerHttpMethod.Get;

        public Task<HttpResponse> HandleAsync(HttpRequestDto request)
        {
            return Task.FromResult(HttpResponse.Create(HttpStatusCode.OK).As(ContentType.ApplicationJson).WithData(new ResponseObject("OK", 200)));
        }
    }
}
