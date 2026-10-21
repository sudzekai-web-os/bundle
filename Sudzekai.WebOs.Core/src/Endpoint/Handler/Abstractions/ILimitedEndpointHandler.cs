namespace Sudzekai.WebOs.Core
{
    public interface ILimitedEndpointHandler : IEndpointHandler
    {
        RateLimitingPolicy RateLimitingPolicy { get; }
    }
}
