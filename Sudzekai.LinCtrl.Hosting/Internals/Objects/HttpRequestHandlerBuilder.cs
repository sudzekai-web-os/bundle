using Sudzekai.LinCtrl.Abi.Managed;
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

        public IRequestHandler Handler { get; }

        public RequestHandlerMetadata? Metadata { get; init; }

        public HttpRequestHandlerBuilder(IRequestHandler handler)
        {
            Handler = handler;
        }

        public string GetTag()
            => $"{Handler.HttpMethod.ToMethodString()}:/{Handler.Pattern.TrimStart('/')}";
    }
}
