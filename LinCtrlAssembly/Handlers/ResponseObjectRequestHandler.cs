using Sudzekai.LinCtrl.Assembly.Extensions;
using Sudzekai.LinCtrl.Abi.Managed;
using LinCtrlAssembly.Objects;
using Sudzekai.LinCtrl.Core;
using System.Net;

namespace LinCtrlAssembly.Handlers
{
    internal class ResponseObjectRequestHandler : IRequestHandler
    {
        public string Pattern => "object";

        public HttpRequestMethod HttpMethod => HttpRequestMethod.Get;

        public bool IsFiltering => true;

        public HttpResponseDto Handle(HttpRequestDto request)
        {
            return HttpResponseDto.Create(HttpStatusCode.OK).As(ResponseContentType.ApplicationJson).WithData(new ResponseObject("OK", 200));
        }
    }
}
