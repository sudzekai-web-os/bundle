using Microsoft.AspNetCore.Builder;
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Hosting
{
    internal class HttpRequestHandlerBuilder
    {
        public IEndpointConventionBuilder ConventionBuilder
        {
            get => field ?? throw new InvalidOperationException("Convention Builder was not created"); 
            set;
        }

        public IHttpRequestHandler Handler { get; }

        public HttpRequestHandlerBuilder(IHttpRequestHandler handler)
        {
            Handler = handler;
        }

        public string GetTag()
            => $"{Handler.HttpMethod.ToMethodString()}:/{Handler.Pattern.TrimStart('/')}";
    }
}
