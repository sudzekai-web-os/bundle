using Sudzekai.LinCtrl.Abi.Managed;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Sudzekai.LinCtrl.Assembly.Context;
using Sudzekai.LinCtrl.Assembly.Internals.Objects;

namespace Sudzekai.LinCtrl.Assembly
{
    public class AssemblyHost : IDisposable, IAsyncDisposable
    {
        private readonly object _sync = new();
        private IServiceScope? _scope;
        private IServiceProvider? _services;
        private IRequestHandler[]? _handlersCache;
        private bool _disposed;

        internal AssemblyHost(IServiceProvider provider)
        {
            _services = provider;
        }

        public IServiceProvider Services
        {
            get
            {
                ObjectDisposedException.ThrowIf(_disposed, this);
                return _services!;
            }
        }

        public static AssemblyHostBuilder CreateBuilder(AssemblyContext ctx)
            => new(ctx);

        public IRequestHandler[] GetHandlers()
        {
            lock (_sync)
            {
                ObjectDisposedException.ThrowIf(_disposed, this);

                if (_handlersCache != null)
                    return [.. _handlersCache];

                var scope = Services.CreateScope();
                try
                {
                    var factory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                    _handlersCache = [.. scope.ServiceProvider.GetServices<IRequestHandler>()
                        .Select(h => (IRequestHandler)new HttpRequestHandlerDecorator(
                            h, factory.CreateLogger(h.GetType().FullName!)))];
                    _scope = scope;
                    return [.. _handlersCache];
                }
                catch
                {
                    if (scope is IAsyncDisposable asyncScope)
                        asyncScope.DisposeAsync().AsTask().GetAwaiter().GetResult();
                    else
                        scope.Dispose();
                    throw;
                }
            }
        }

        public string GetHandlersMetadata()
        {
            var metadata = GetHandlers().Select(handler => handler.Metadata).OfType<RequestHandlerMetadata>().ToArray();
            return metadata.Length == 0
                ? string.Empty
                : JsonSerializer.Serialize(metadata, RequestHandlerMetadataContext.Default.RequestHandlerMetadataArray);
        }

        public void Dispose()
            => DisposeAsync().AsTask().GetAwaiter().GetResult();

        public async ValueTask DisposeAsync()
        {
            IServiceScope? scope;
            IServiceProvider? services;
            lock (_sync)
            {
                if (_disposed)
                    return;

                _disposed = true;
                _handlersCache = null;
                scope = _scope;
                services = _services;
                _scope = null;
                _services = null;
            }

            try
            {
                if (scope is IAsyncDisposable asyncScope)
                    await asyncScope.DisposeAsync().ConfigureAwait(false);
                else
                    scope?.Dispose();
            }
            finally
            {
                if (services is IAsyncDisposable asyncServices)
                    await asyncServices.DisposeAsync().ConfigureAwait(false);
                else
                    (services as IDisposable)?.Dispose();
            }
        }
    }
}
