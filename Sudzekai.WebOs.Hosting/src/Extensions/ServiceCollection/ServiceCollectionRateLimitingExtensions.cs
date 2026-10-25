using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Sudzekai.WebOs.Core;

namespace Sudzekai.WebOs.Hosting
{
    public static class ServiceCollectionRateLimitingExtensions
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection ConfigureRateLimiting(Action<RateLimitingOptions> options)
            {
                RateLimitingOptions limitingOptions = new();

                options(limitingOptions);

                return services.ConfigureRateLimiting(limitingOptions);
            }

            public IServiceCollection ConfigureRateLimiting(RateLimitingOptions? options = null)
                => services.AddRateLimiter(o =>
                {
                    o.AddPolicy(
                        RateLimitingPolicy.Default.ToString(),
                        options?.Default ?? RateLimitingPoliciesDefaults.Default);

                    o.AddPolicy(
                        RateLimitingPolicy.Anonymous.ToString(),
                        options?.Anonymous ?? RateLimitingPoliciesDefaults.Anonymous);

                    o.AddPolicy(
                        RateLimitingPolicy.Authenticated.ToString(),
                        options?.Authenticated ?? RateLimitingPoliciesDefaults.Authenticated);

                    o.AddPolicy(
                        RateLimitingPolicy.Heavy.ToString(),
                        options?.Heavy ?? RateLimitingPoliciesDefaults.Heavy);

                    o.AddPolicy(
                        RateLimitingPolicy.Strict.ToString(),
                        options?.Strict ?? RateLimitingPoliciesDefaults.Strict);
                });
        }
    }
}
