using System.Collections.Specialized;
using System.Web;

namespace Sudzekai.WebOs.Core
{
    public class HandlerRequest
    {
        public HandlerHttpMethod HttpMethod { get; set; } = HandlerHttpMethod.Get;

        public string? Host { get; set; }

        public string Scheme { get; set; } = string.Empty;

        public string Path { get; set; } = "/";

        public string QueryString { get; set; } = string.Empty;

        public Dictionary<string, string[]> Headers { get; set; } = [];

        public Dictionary<string, string> RouteValues { get; set; } = [];

        public Dictionary<string, string> Cookies { get; set; } = [];

        public Dictionary<string, string[]> Claims { get; set; } = [];

        public string? ContentType { get; set; }

        public byte[]? Body { get; set; }
    }
}