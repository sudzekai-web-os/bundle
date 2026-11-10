using Sudzekai.LinCtrl.Assembly.Extensions;
using Sudzekai.LinCtrl.Abi.Managed;
using LinCtrlAssembly.Objects;
using Sudzekai.LinCtrl.Core;

namespace LinCtrlAssembly.Handlers
{
    internal class UnregisteredResponseObjectHandler : IRequestHandler
    {
        public string Pattern => "object/error";

        public HttpRequestMethod HttpMethod => HttpRequestMethod.Get;

        public HttpResponseDto Handle(HttpRequestDto request)
        {
            return HttpResponseDto.Create(System.Net.HttpStatusCode.OK).As(ResponseContentType.ApplicationJson).WithData(new UnregisteredResponseObject());
        }
    }
}
