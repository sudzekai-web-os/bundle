using Sudzekai.LinCtrl.Abi.Managed;
using Microsoft.AspNetCore.Builder;
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Hosting
{
    internal class HttpRequestHandlerBuilder(
        object registration,
        string pattern,
        HttpRequestMethod httpMethod,
        bool isFiltering,
        Func<IServiceProvider, IRequestHandler> resolve)
    {
        public IEndpointConventionBuilder ConventionBuilder
        {
            get => field ?? throw new InvalidOperationException("Convention Builder was not created");
            set;
        }

        public object Registration => registration;
        public string Pattern => pattern;
        public HttpRequestMethod HttpMethod => httpMethod;
        public bool IsFiltered => isFiltering;
        public string? OpenApiOperation { get; set; }
        public Action<EndpointBuilder>? Configure { get; init; }
        public int? MaxRequestBodyBytes { get; init; }

        public HttpRequestHandlerBuilder(IRequestHandler handler)
            : this(handler, handler.Pattern, handler.HttpMethod, handler.IsFiltered, _ => handler)
        {
        }

        public IRequestHandler Resolve(IServiceProvider services)
            => resolve(services);

        public string GetTag()
            => $"{HttpMethod.ToMethodString()}:/{Pattern.TrimStart('/')}";
    }
}
