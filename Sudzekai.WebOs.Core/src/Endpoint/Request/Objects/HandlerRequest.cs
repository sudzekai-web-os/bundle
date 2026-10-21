namespace Sudzekai.WebOs.Core
{
    public record HandlerRequest
    {
        public HandlerHttpMethod HttpMethod { get; set; } = HandlerHttpMethod.Get;

        public string Path { get; set; } = "/";

        public string QueryString { get; set; } = string.Empty;

        public IReadOnlyCollection<HeaderEntry> Headers { get; set; } = [];

        public byte[]? Body { get; set; }
    }
}