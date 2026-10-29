using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sudzekai.WebOs.Core;
using System.Text.Json;

namespace Sudzekai.WebOs.Plugin
{

    internal sealed class EndpointHandlerManager : IEndpointHandlerManager
    {
        private readonly IServiceProvider _services;

        private EndpointHandlerDescriptor[] _descriptors = [];
        private readonly List<IEndpointHandler> _handlers = [];

        public EndpointHandlerManager(IServiceProvider services)
        {
            _services = services;
        }

        public IReadOnlyList<IEndpointHandler> Handlers => _handlers;

        public EndpointHandlerDescriptor[] Descriptors => _descriptors;

        public void Load(IReadOnlyList<Type> handlerTypes)
        {
            Release();

            _handlers.Clear();
            _descriptors = [];

            foreach (var type in handlerTypes)
            {
                var handler = (IEndpointHandler)_services.GetRequiredService(type);

                _handlers.Add(handler);

                var descriptor = handler.ToDescriptor();

                descriptor.SetHandler(DecorateHandleFunc(type));

                _descriptors = [.. _descriptors.Append(descriptor)];
            }
        }

        private Func<HandlerRequest, Task<HandlerResult>> DecorateHandleFunc(Type type)
            => (request) =>
            {
                var logger = _services.GetRequiredService<ILoggerFactory>().CreateLogger(type.ToString());

                var handler = (IEndpointHandler)_services.GetRequiredService(type)!;

                var result = handler.HandleAsync(request).GetAwaiter().GetResult();

                if (result.ContentType is ContentType.ApplicationJson)
                {
                    if (result.JsonTypeInfo is null)
                        throw new InvalidOperationException($"у метода обработки обработчика типа {type} возвращаемый тип данных - application/json, но JsonTypeInfo не был указан");

                    var resultData = JsonSerializer.Serialize(result.Data, result.JsonTypeInfo);

                    result.WithData(resultData);
                }

                return Task.FromResult(result);
            };

        public void Release()
        {
            foreach (var descriptor in _descriptors)
                descriptor.Release();
        }
    }
}