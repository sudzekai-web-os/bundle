using Sudzekai.LinCtrl.Extensions;
using System.Text.Json;
using Sudzekai.LinCtrl.Abi.Managed;
using Microsoft.Extensions.Logging;
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Hosting
{
    internal class RequestHandlerLifetimeHost(
        ILogger<RequestHandlerLifetimeHost> logger,
        IEnumerable<IRequestHandlerBuilder> builders,
        RequestHandlerEndpointDataSource dataSource)
    {
        public void Remove(IEnumerable<IRequestHandler> handlers)
            => dataSource.Remove(handlers);

        public void Build(IRequestHandler handler, JsonElement? paths, HostRequestHandlerRegistration? registration = null)
        {
            var builder = registration == null
                ? new HttpRequestHandlerBuilder(handler)
                : new HttpRequestHandlerBuilder(registration, handler.Pattern,
                    handler.HttpMethod, handler.IsFiltered, registration.Resolve)
                {
                    Configure = registration.Configure,
                    MaxRequestBodyBytes = registration.MaxRequestBodyBytes
                };

            var path = OpenApiMetadataWriter.NormalizePath(handler.Pattern);
            var method = handler.HttpMethod.ToString().ToLowerInvariant();
            if (paths is { } operations && operations.TryGetProperty(path, out var methods)
                && methods.TryGetProperty(method, out var operation))
            {
                builder.OpenApiOperation = operation.GetRawText();
            }
            else if (handler.Metadata != null || handler.IsFiltered)
            {
                using var document = JsonDocument.Parse(new[] { handler }.WriteOpenApiMetadata());
                builder.OpenApiOperation = document.RootElement.GetProperty("paths")
                    .GetProperty(path).GetProperty(method).GetRawText();
            }

            Build(builder);
        }

        private HttpRequestHandlerBuilder Build(HttpRequestHandlerBuilder builder)
        {
            logger.LogInformation("[{tag}] Handler building started", builder.GetTag());

            try
            {
                foreach (var initializer in builders)
                    initializer.Build(builder);
            }
            catch
            {
                dataSource.Remove([builder.Registration]);
                throw;
            }

            logger.LogInformation("[{tag}] Handler built", builder.GetTag());

            return builder;
        }
    }
}
