using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Abi.Managed
{
    public class HttpRequestDto
    {
        public HttpRequestMethod HttpMethod { get; set; }

        public string Host { get; set; } = string.Empty;

        public string Scheme { get; set; } = string.Empty;

        public string Path { get; set; } = string.Empty;

        public string QueryString { get; set; } = string.Empty;

        public Dictionary<string, string[]> Headers { get; set; } = [];

        public Dictionary<string, string[]> Claims { get; set; } = [];

        public Dictionary<string, string> RouteValues { get; set; } = [];

        public Dictionary<string, string> Cookies { get; set; } = [];

        public ResponseContentType ContentType { get; set; }

        public byte[] Body { get; set; } = [];
    }
}
