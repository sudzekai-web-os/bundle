using Sudzekai.LinCtrl.Assembly.Extensions;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Core;

namespace LinCtrlAssembly.Handlers
{
    internal class ResponseStringRequestHandler : IRequestHandler
    {
        public string Pattern => "string";

        public HttpRequestMethod HttpMethod => HttpRequestMethod.Get;

        public HttpResponseDto Handle(HttpRequestDto request)
        {
            return HttpResponseDto.Create(System.Net.HttpStatusCode.OK).As(ResponseContentType.ApplicationJson).WithData("string");
        }
    }
}
