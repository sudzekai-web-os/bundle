using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sudzekai.WebOs.Core;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace Sudzekai.WebOs.Plugin
{
    public class WebOsPlugin
    {
        private AssemblyInfo _pluginInfo;

        private EndpointHandlerDescriptor[] _handlerDescriptors = [];

        private readonly List<IEndpointHandler> _handlers = [];

        public readonly ServiceProvider Services;

        internal WebOsPlugin(
            ServiceProvider services,
            List<Type> handlerTypes,
            AssemblyInfo? info)
        {
            _pluginInfo = info ?? new();

            Services = services;
            LoadHandlerDescriptors(handlerTypes);
        }

        public unsafe EndpointHandlerDescriptor* GetHandlers(int* count)
        {
            *count = _handlerDescriptors.Length;
            if (_handlerDescriptors.Length == 0)
                return null;

            int size = Marshal.SizeOf<EndpointHandlerDescriptor>();

            var mem = (EndpointHandlerDescriptor*)Marshal.AllocCoTaskMem(size * *count);

            for (int i = 0; i < *count; i++)
                Marshal.StructureToPtr(_handlerDescriptors[i], (nint)(mem + i), fDeleteOld: false);

            return mem;
        }

        public AssemblyInfoDescriptor GetPluginInfo()
            => _pluginInfo.ToDescriptor();

        public unsafe nint GetHandlersMetadata(int* length)
        {
            var json = HandlerOpenApiMetadataWriter.Write(_handlers);

            return AllocationUtilities.AllocString(json, out *length);
        }

        private void LoadHandlerDescriptors(List<Type> handlerTypes)
        {
            foreach (var type in handlerTypes)
            {
                var temp = (IEndpointHandler)Services.GetRequiredService(type);

                _handlers.Add(temp);

                var descriptor = temp.ToDescriptor();

                descriptor.SetHandler(DecorateHandleFunc(type));

                _handlerDescriptors = [.. _handlerDescriptors.Append(descriptor)];
            }
        }

        private Func<HandlerRequest, Task<HandlerResult>> DecorateHandleFunc(Type type)
            => (request) =>
            {
                var logger = Services.GetRequiredService<ILoggerFactory>().CreateLogger(type.ToString());

                var handler = (IEndpointHandler)Services.GetRequiredService(type)!;

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

        public int Dispose()
        {
            foreach (var handlerDescriptor in _handlerDescriptors)
                handlerDescriptor.Release();

            return 0;
        }
    }
}
