using Microsoft.AspNetCore.Http;
using System.Threading.RateLimiting;

namespace Sudzekai.WebOs.Hosting
{
    internal class RateLimitingPoliciesDefaults
    {
        public static Func<HttpContext, RateLimitPartition<string>> Default
            => ctx =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    }
                );

        public static Func<HttpContext, RateLimitPartition<string>> Anonymous
            => ctx =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: "anon:" + (ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown"),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 30,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    }
                );

        public static Func<HttpContext, RateLimitPartition<string>> Authenticated
            => ctx =>
                RateLimitPartition.GetSlidingWindowLimiter(
                    partitionKey: ctx.User.Identity?.IsAuthenticated == true
                        ? "user:" + ctx.User.Identity.Name
                        : "ip:" + (ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown"),
                    factory: _ => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = 300,
                        Window = TimeSpan.FromMinutes(1),
                        SegmentsPerWindow = 6,
                        QueueLimit = 0
                    }
                );

        public static Func<HttpContext, RateLimitPartition<string>> Heavy
            => ctx =>
                RateLimitPartition.GetTokenBucketLimiter(
                    partitionKey: ctx.User.Identity?.IsAuthenticated == true
                        ? "user:" + ctx.User.Identity.Name
                        : "ip:" + (ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown"),
                    factory: _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 10,
                        TokensPerPeriod = 5,
                        ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }
                );

        public static Func<HttpContext, RateLimitPartition<string>> Strict 
            => ctx =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: "strict:" + (ctx.User.Identity?.IsAuthenticated == true
                        ? "user:" + ctx.User.Identity.Name
                        : "ip:" + (ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown")),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    }
                );
    }
}
