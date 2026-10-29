using Microsoft.Extensions.DependencyInjection;
using Sudzekai.WebOs.Core;
using System.Runtime.InteropServices;

namespace Sudzekai.WebOs.Plugin
{
    public class WebOsPlugin
    {
        private readonly IEndpointHandlerManager _handlerManager;

        private readonly IOpenApiMetadataWriter _metadataWriter;

        private AssemblyInfo _pluginInfo;

        public readonly ServiceProvider Services;

        internal WebOsPlugin(
            ServiceProvider services,
            List<Type> handlerTypes,
            AssemblyInfo? info)
        {
            _pluginInfo = info ?? new();

            Services = services;
            _handlerManager = services.GetRequiredService<IEndpointHandlerManager>();
            _metadataWriter = services.GetRequiredService<IOpenApiMetadataWriter>();
            _handlerManager.Load(handlerTypes);
        }

        public unsafe EndpointHandlerDescriptor* GetHandlers(int* count)
        {
            var descriptors = _handlerManager.Descriptors;

            *count = descriptors.Length;
            if (descriptors.Length == 0)
                return null;

            int size = Marshal.SizeOf<EndpointHandlerDescriptor>();

            var mem = (EndpointHandlerDescriptor*)Marshal.AllocCoTaskMem(size * *count);

            for (int i = 0; i < *count; i++)
                Marshal.StructureToPtr(descriptors[i], (nint)(mem + i), fDeleteOld: false);

            return mem;
        }

        public AssemblyInfoDescriptor GetPluginInfo()
            => _pluginInfo.ToDescriptor();

        public unsafe nint GetHandlersMetadata(int* length)
        {
            var json = _metadataWriter.Write(_handlerManager.Handlers);

            return AllocationUtilities.AllocString(json, out *length);
        }

        public int Dispose()
        {
            _handlerManager.Release();

            return 0;
        }
    }
}