using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Abi.Managed
{
    public interface IRequestHandler
    {
        public string Pattern { get; }

        public HttpRequestMethod HttpMethod { get; }

        public HttpResponseDto Handle(HttpRequestDto request);
    }
}
