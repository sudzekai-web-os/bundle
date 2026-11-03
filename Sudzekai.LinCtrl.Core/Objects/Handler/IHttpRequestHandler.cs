
namespace Sudzekai.LinCtrl.Core 
{
    public interface IHttpRequestHandler
    {
        string Pattern { get; }

        RequestHandlerHttpMethod HttpMethod { get; }

        Task<HttpResponse> HandleAsync(HttpRequestDto request);
    }
}
