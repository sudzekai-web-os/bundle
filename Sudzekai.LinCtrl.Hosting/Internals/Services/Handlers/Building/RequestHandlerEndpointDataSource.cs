using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.Primitives;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Hosting
{
    internal sealed class RequestHandlerEndpointDataSource : EndpointDataSource, IDisposable
    {
        private readonly object _sync = new();
        private readonly Dictionary<object, HandlerRegistration> _handlers = [];
        private CancellationTokenSource _changes = new();
        private bool _disposed;

        public override IReadOnlyList<Endpoint> Endpoints
        {
            get
            {
                lock (_sync)
                    return [.. _handlers.Values.Select(h => h.Build())];
            }
        }

        public IReadOnlyList<(string Pattern, string Method, string Operation)> GetOpenApiOperations()
        {
            lock (_sync)
                return [.. _handlers.Values.Where(handler => handler.OpenApiOperation != null)
                    .Select(handler => (handler.Pattern, handler.Method, handler.OpenApiOperation!))];
        }

        public override IChangeToken GetChangeToken()
        {
            lock (_sync)
                return new CancellationChangeToken(_disposed ? CancellationToken.None : _changes.Token);
        }

        public IEndpointConventionBuilder Add(IRequestHandler handler)
            => Add(handler, handler.Pattern, handler.HttpMethod);

        public IEndpointConventionBuilder Add(object key, string pattern, HttpRequestMethod method, string? openApiOperation = null)
        {
            var registration = new HandlerRegistration(this, RoutePatternFactory.Parse(pattern), method.ToMethodString(), openApiOperation);
            Update(() => _handlers.Add(key, registration));
            return registration;
        }

        public void Remove(IEnumerable<object> handlers)
        {
            Update(() =>
            {
                foreach (var handler in handlers)
                {
                    if (_handlers.Remove(handler, out var registration))
                        registration.Clear();
                }
            }, true);
        }

        private void Update(Action action, bool allowDisposed = false)
        {
            CancellationTokenSource changes;
            lock (_sync)
            {
                if (_disposed && allowDisposed)
                    return;
                ObjectDisposedException.ThrowIf(_disposed, this);
                action();
                changes = _changes;
                _changes = new();
            }

            try
            {
                changes.Cancel();
            }
            finally
            {
                changes.Dispose();
            }
        }

        public void Dispose()
        {
            CancellationTokenSource changes;
            lock (_sync)
            {
                if (_disposed)
                    return;

                _disposed = true;
                foreach (var registration in _handlers.Values)
                    registration.Clear();
                _handlers.Clear();
                changes = _changes;
            }

            changes.Dispose();
        }

        private sealed class HandlerRegistration(
            RequestHandlerEndpointDataSource source,
            RoutePattern pattern,
            string method,
            string? openApiOperation) : IEndpointConventionBuilder
        {
            private readonly List<Action<EndpointBuilder>> _conventions = [];
            private readonly List<Action<EndpointBuilder>> _finalConventions = [];

            public string Pattern => pattern.RawText!;
            public string Method => method;
            public string? OpenApiOperation => openApiOperation;

            public void Add(Action<EndpointBuilder> convention)
                => source.Update(() => _conventions.Add(convention));

            public void Finally(Action<EndpointBuilder> convention)
                => source.Update(() => _finalConventions.Add(convention));

            public Endpoint Build()
            {
                var builder = new RouteEndpointBuilder(_ => Task.CompletedTask, pattern, 0);
                builder.Metadata.Add(new HttpMethodMetadata([method]));

                foreach (var convention in _conventions)
                    convention(builder);
                foreach (var convention in _finalConventions)
                    convention(builder);

                return builder.Build();
            }

            public void Clear()
            {
                _conventions.Clear();
                _finalConventions.Clear();
            }
        }
    }
}
