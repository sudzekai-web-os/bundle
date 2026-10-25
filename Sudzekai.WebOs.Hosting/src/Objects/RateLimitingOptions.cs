using Microsoft.AspNetCore.Http;
using System.Threading.RateLimiting;

namespace Sudzekai.WebOs.Hosting
{
    public class RateLimitingOptions
    {
        public Func<HttpContext, RateLimitPartition<string>>? Default { get; set; }

        public Func<HttpContext, RateLimitPartition<string>>? Anonymous { get; set; }
        
        public Func<HttpContext, RateLimitPartition<string>>? Authenticated { get; set; }
        
        public Func<HttpContext, RateLimitPartition<string>>? Heavy { get; set; }
        
        public Func<HttpContext, RateLimitPartition<string>>? Strict { get; set; }
    }
}
