using Sudzekai.WebOs.Core;

namespace Sudzekai.WebOs.Plugin
{
    public static class EndpointRequestExtensions
    {
        public static string[] GetHeaderValues(this HandlerRequest request, string key)
            => request.Headers.FirstOrDefault(h => h.Key == key)?.Values ?? [];
    }
}
