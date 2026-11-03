
namespace Sudzekai.LinCtrl.Core 
{
    internal class HttpRequestHandler : IHttpRequestHandler
    {
        public HttpRequestHandler(string pattern, RequestHandlerHttpMethod httpMethod, RequestHandleDelegate handleFunc)
        {
            Pattern = pattern;
            HttpMethod = httpMethod;
            HandleFunc = handleFunc;
        }

        public string Pattern { get; set; }

        public RequestHandlerHttpMethod HttpMethod { get; set; }

        public RequestHandleDelegate HandleFunc { get; set; }

        public Task<HttpResponse> HandleAsync(HttpRequestDto request)
            => HandleFunc(request);
    }
}
