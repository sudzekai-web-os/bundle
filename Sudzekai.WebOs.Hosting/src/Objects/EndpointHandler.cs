using Sudzekai.WebOs.Core;

namespace Sudzekai.WebOs.Hosting
{
    public class EndpointHandler
    {
        public string Pattern { get; internal set; } = string.Empty;

        public HandlerHttpMethod HttpMethod { get; internal set; }

        internal Func<HandlerRequest, HandlerResult> HandleFunc { get; set; } = default!;

        internal async Task<HandlerResult> HandleAsync(HandlerRequest request)
        {
            return HandleFunc(request);
        }

        // Authorized
        public bool IsAuthorizedHandler { get; internal set; }

        public ClientRole[] RequiredRoles { get; internal set; } = [];


        // Filtered
        public bool IsFilteredHandler { get; internal set; }


        // RateLimitng
        public bool IsLimitedHandler { get; internal set; }

        public RateLimitingPolicy RateLimitingPolicy { get; internal set; }


        // OpenApi
        public bool IsOpenApiHandler { get; internal set; }

        public string Name { get; internal set; } = string.Empty;

        public string DisplayName { get; internal set; } = string.Empty;

        public string[] Tags { get; internal set; } = [];

        public string Summary { get; internal set; } = string.Empty;

        public string GroupName { get; internal set; } = string.Empty;


        // Routed
        public bool IsRoutedHandler { get; internal set; }

        public int Priority { get; internal set; }

        public string[] Hosts { get; internal set; } = [];
    }
}
