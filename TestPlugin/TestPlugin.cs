using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sudzekai.WebOs.Core;
using Sudzekai.WebOs.Plugin;
using System.Runtime.InteropServices;

namespace TestPlugin
{
    public class TestPlugin
    {
        private static WebOsPluginBuilder _builder = default!;
        private static WebOsPlugin _plugin = default!;
        

        [UnmanagedCallersOnly(EntryPoint = WebOsCoreContext.Exports.Main)]
        public static int Main()
        {
            _builder = WebOsPluginBuilder.NewBuilder();
            _builder.SetInformation(new()
            {
                Name = "TestPlugin",
                Version = "0.0.1",
                Summary = "Тестовый плагин предоставляющий роут /health"
            });
            _builder.Services.AddScoped<TestHandler>();

            return 0;
        }


        [UnmanagedCallersOnly(EntryPoint = WebOsCoreContext.Exports.DelegateLogging)]
        public static int DelegateLogging(nint logAction)
            => _builder.DelegateLogging(logAction);


        [UnmanagedCallersOnly(EntryPoint = WebOsCoreContext.Exports.Build)]
        public static int Build()
        {
            _plugin = _builder.Build();

            var logger = _plugin.Services.GetRequiredService<ILogger<TestPlugin>>();

            return 0;
        }

        [UnmanagedCallersOnly(EntryPoint = WebOsCoreContext.Exports.GetHandlers)]
        public static unsafe EndpointHandlerDescriptor* GetHandlers(int* count)
            => _plugin.GetHandlers(count);


        [UnmanagedCallersOnly(EntryPoint = WebOsCoreContext.Exports.GetInfo)]
        public static PluginInfoDescriptor GetPluginInfo()
            => _plugin.GetPluginInfo();


        [UnmanagedCallersOnly(EntryPoint = WebOsCoreContext.Exports.Dispose)]
        public static int Dispose()
            => _plugin.Dispose();
    }
}

