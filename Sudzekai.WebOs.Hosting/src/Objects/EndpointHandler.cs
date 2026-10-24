using Sudzekai.WebOs.Core;

namespace Sudzekai.WebOs.Hosting
{
    public class EndpointHandler : IEndpointHandler
    {
        internal EndpointHandler(string pattern, HandlerHttpMethod method, Func<HandlerRequest, HandlerResult> handleFunc)
        {
            Pattern = pattern;
            HttpMethod = method;
            HandleFunc = handleFunc;
        }

        internal void WithAuthorization(HandlerAuthorizationInfo? info)
            => AuthorizationInfo = info;

        internal void WithFilter(HandlerFilteringInfo? info)
            => FilteringInfo = info;

        internal void WithLimiting(HandlerLimitingInfo? info)
            => LimitingInfo = info;

        internal void WithOpenApi(HandlerOpenApiInfo? info)
            => OpenApiInfo = info;

        internal void WithRouting(HandlerRoutingInfo? info)
            => RoutingInfo = info;

        public string Pattern { get; }

        public HandlerHttpMethod HttpMethod { get; }

        public HandlerAuthorizationInfo? AuthorizationInfo { get; private set; }

        public HandlerFilteringInfo? FilteringInfo { get; private set; }

        public HandlerLimitingInfo? LimitingInfo { get; private set; }

        public HandlerOpenApiInfo? OpenApiInfo { get; private set; }

        public HandlerRoutingInfo? RoutingInfo { get; private set; }

        internal Func<HandlerRequest, HandlerResult> HandleFunc { get; }

        public Task<HandlerResult> HandleAsync(HandlerRequest request)
            => Task.FromResult(HandleFunc(request));
    }
}
