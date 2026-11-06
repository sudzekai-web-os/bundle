using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sudzekai.LinCtrl.Assembly.Context;
using Sudzekai.LinCtrl.Assembly.Internals.Objects;
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Assembly
{
    public class AssemblyHost
    {
        private List<IHttpRequestHandler> _handlersCache = [];
        
        internal AssemblyHost(IServiceProvider provider)
        {
            Services = provider;
        }

        public IServiceProvider Services;

        public static AssemblyHostBuilder CreateBuilder(AssemblyContext ctx)
            => new(ctx);

        public IHttpRequestHandler[] GetHandlers()
        {
            var factory = Services.GetRequiredService<ILoggerFactory>();

            if (_handlersCache.Count == 0)
                _handlersCache = [.. Services.GetServices<IHttpRequestHandler>()];

            return [.. _handlersCache
                    .Select(h => (IHttpRequestHandler)new HttpRequestHandlerDecorator(
                        h, factory.CreateLogger(h.GetType().FullName!)))];
        }

        public string GetHandlersMetadata()
        {
            

            return string.Empty;
        }
    }
}
