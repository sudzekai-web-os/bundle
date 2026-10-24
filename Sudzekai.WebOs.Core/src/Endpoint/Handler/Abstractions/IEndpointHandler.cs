namespace Sudzekai.WebOs.Core
{
    public interface IEndpointHandler
    {
        string Pattern { get; }

        HandlerHttpMethod HttpMethod { get; }

        HandlerAuthorizationInfo? AuthorizationInfo { get => null; }

        HandlerFilteringInfo? FilteringInfo { get => null; }

        HandlerLimitingInfo? LimitingInfo { get => null; }

        HandlerOpenApiInfo? OpenApiInfo { get => null; }

        HandlerRoutingInfo? RoutingInfo { get => null; }

        Task<HandlerResult> HandleAsync(HandlerRequest request);
    }
}
