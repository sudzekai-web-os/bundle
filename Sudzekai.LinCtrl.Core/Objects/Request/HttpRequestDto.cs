
namespace Sudzekai.LinCtrl.Core 
{
    public class HttpRequestDto
    {
        public RequestHandlerHttpMethod HttpMethod { get; set; }

        public string Host { get; set; } = string.Empty;

        public string Scheme { get; set; } = string.Empty;

        public string Path { get; set; } = "/";

        public string QueryString { get; set; } = string.Empty;

        public Dictionary<string, string[]> Claims { get; set; } = [];
        
        public Dictionary<string, string[]> Headers { get; set; } = [];

        public Dictionary<string, string> RouteValues { get; set; } = [];

        public Dictionary<string, string> Cookies { get; set; } = [];

        public ContentType ContentType { get; set; }

        public byte[] Body { get; set; } = [];
    }
}
