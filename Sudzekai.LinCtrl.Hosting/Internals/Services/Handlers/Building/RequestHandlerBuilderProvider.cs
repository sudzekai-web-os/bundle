using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Sudzekai.LinCtrl.Hosting
{
    internal class RequestHandlerBuilderProvider(ILogger<RequestHandlerBuilderProvider> logger) : IRequestHandlerBuilder
    {
        public virtual IEndpointRouteBuilder RouteBuilder
        {
            get => field ?? throw new InvalidOperationException("Endpoint Route Builder was not properly set");
            private set;
        }

        public void Build(HttpRequestHandlerBuilder builder)
        {
            if (RouteBuilder == null)
                throw new InvalidOperationException("Route builder was not properly installed");

            logger.LogDebug("[{tag}] Creating handler builder...", builder.GetTag());

            builder.ConventionBuilder = RouteBuilder.MapMethods(
                builder.Handler.Pattern,
                [builder.Handler.HttpMethod.ToMethodString()],
                () => { });

            logger.LogDebug("[{tag}] Handler builder created", builder.GetTag());
        }

        public virtual void SetRouteBuilder(IEndpointRouteBuilder builder)
            => RouteBuilder = builder;
    }
}
