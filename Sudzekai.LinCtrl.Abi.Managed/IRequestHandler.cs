using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Abi.Managed
{
    public interface IRequestHandler
    {
        public string Pattern { get; }

        public HttpRequestMethod HttpMethod { get; }

        public bool IsFiltered { get => true; }

        public HandlerMetadata? Metadata { get => null; }

        public HttpResponseDto Handle(HttpRequestDto request);
    }
}
