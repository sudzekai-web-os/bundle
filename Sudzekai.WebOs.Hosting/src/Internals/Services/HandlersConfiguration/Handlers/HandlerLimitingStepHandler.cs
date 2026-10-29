using Microsoft.AspNetCore.Builder;
using Sudzekai.WebOs.Core;

namespace Sudzekai.WebOs.Hosting
{
    internal class HandlerLimitingStepHandler : IHandlerConfigurationStepHandler
    {
        public string Handle(IEndpointConventionBuilder builder, IEndpointHandler handler)
        {
            var policy = RateLimitingPolicy.Default.ToString();

            if (handler.LimitingInfo is { } info)
                policy = info.RateLimitingPolicy.ToString();

            builder.RequireRateLimiting(policy);

            return LogsDictionary.HandlerLimitingStepHandler.RateLimitingAdded.Replace("{policy}", policy);
        }
    }
}
