namespace Sudzekai.WebOs.Core
{
    public class HandlerLimitingInfo
    {
        public HandlerLimitingInfo(RateLimitingPolicy rateLimitingPolicy)
        {
            RateLimitingPolicy = rateLimitingPolicy;
        }

        public RateLimitingPolicy RateLimitingPolicy { get; }
    }
}
