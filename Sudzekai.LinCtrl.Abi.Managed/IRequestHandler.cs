using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Abi.Managed
{
    public interface IRequestHandler
    {
        public string Pattern { get; }

        public HttpRequestMethod HttpMethod { get; }

        public bool IsFiltering => false;

        public RequestHandlerMetadata? Metadata => null;

        public HttpResponseDto Handle(HttpRequestDto request);
    }
}
