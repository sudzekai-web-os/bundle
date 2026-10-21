namespace Sudzekai.WebOs.Core
{
    public interface IEndpointHandler
    {
        string Pattern { get; }

        HandlerHttpMethod HttpMethod { get; }

        Task<HandlerResult> Handle(HandlerRequest request);
    }
}
