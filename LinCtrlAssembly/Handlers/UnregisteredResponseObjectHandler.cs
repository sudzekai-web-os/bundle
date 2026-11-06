using LinCtrlAssembly.Objects;
using Sudzekai.LinCtrl.Core;

namespace LinCtrlAssembly.Handlers
{
    internal class UnregisteredResponseObjectHandler : IHttpRequestHandler
    {
        public string Pattern => "object/error";

        public HttpRequestMethod HttpMethod => HttpRequestMethod.Get;

        public Task<HttpResponse> HandleAsync(HttpRequestDto request)
        {
            return Task.FromResult(HttpResponse.Create(System.Net.HttpStatusCode.OK).As(ResponseContentType.ApplicationJson).WithData(new UnregisteredResponseObject()));
        }
    }
}
