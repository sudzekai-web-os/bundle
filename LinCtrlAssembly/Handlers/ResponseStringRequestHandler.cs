using Sudzekai.LinCtrl.Core;

namespace LinCtrlAssembly.Handlers
{
    internal class ResponseStringRequestHandler : IHttpRequestHandler
    {
        public string Pattern => "string";

        public HttpRequestMethod HttpMethod => HttpRequestMethod.Get;

        public Task<HttpResponse> HandleAsync(HttpRequestDto request)
        {
            return Task.FromResult(HttpResponse.Create(System.Net.HttpStatusCode.OK).As(ResponseContentType.ApplicationJson).WithData("string"));
        }
    }
}
