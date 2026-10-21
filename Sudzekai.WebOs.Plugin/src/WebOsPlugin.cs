using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sudzekai.WebOs.Core;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace Sudzekai.WebOs.Plugin
{
    public class WebOsPlugin
    {
        private PluginInfo _pluginInfo;

        private EndpointHandlerDescriptor[] _handlerDescriptors = [];

        public readonly ServiceProvider Services;

        internal WebOsPlugin(
            ServiceProvider services,
            List<Type> handlerTypes,
            PluginInfo? info)
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

        public PluginInfoDescriptor GetPluginInfo()
            => _pluginInfo.ToDescriptor();

        private void LoadHandlerDescriptors(List<Type> handlerTypes)
        {
            foreach (var type in handlerTypes)
            {
                var temp = (IEndpointHandler)Services.GetRequiredService(type);

                var descriptor = temp.ToDescriptor();

                descriptor.SetHandler((request) =>
                {
                    var handler = (IEndpointHandler)Services.GetRequiredService(type)!;

                    var logger = Services.GetRequiredService<ILoggerFactory>().CreateLogger(type.ToString());

                    var result = handler.Handle(request).GetAwaiter().GetResult();

                    if (result.ContentType is ContentType.ApplicationJson)
                    {
                        logger.LogInformation("Получен запрос");

                        if (result.JsonTypeInfo is null)
                            throw new InvalidOperationException($"у метода обработки обработчика типа {type} возвращаемый тип данных - application/json, но JsonTypeInfo не был указан");

                        var resultData = JsonSerializer.Serialize(result.Data, result.JsonTypeInfo);

                        result.WithData(resultData);
                    }

                    return Task.FromResult(result);
                });

                _handlerDescriptors = [.. _handlerDescriptors.Append(descriptor)];
            }
        }

        public int Dispose()
        {
            foreach (var handlerDescriptor in _handlerDescriptors)
                handlerDescriptor.Release();

            return 0;
        }
    }
}
