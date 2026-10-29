using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sudzekai.WebOs.Core;

namespace Sudzekai.WebOs.Plugin
{
    public class WebOsPluginBuilder
    {
        private AssemblyInfo? _pluginInfo;

        private WebOsPluginBuilder() { }

        public static WebOsPluginBuilder NewBuilder()
            => new();

        public readonly ServiceCollection Services = new();

        
        public void SetInformation(AssemblyInfo info)
            => _pluginInfo = info;

        public WebOsPlugin Build()
        {
            var provider = Services.BuildServiceProvider();
            Services.MakeReadOnly();

            return new(provider, GetRegisteredHandlerTypes(), _pluginInfo);
        }

        public unsafe int DelegateLogging(nint logActionPtr)
        {
            var logActionDelegate = (delegate* unmanaged<LogEntryDescriptor, void>)logActionPtr;

            Action<LogEntryDescriptor> logAction = (LogEntryDescriptor desc) => logActionDelegate(desc);

            Services.AddLogging(b =>
            {
                b.ClearProviders();
                b.AddProvider(new DelegatedLoggerProvider(logAction));
            });

            return 0;
        }

        private List<Type> GetRegisteredHandlerTypes()
           => [.. Services.Where(sd => sd.ImplementationType != null
               && typeof(IEndpointHandler).IsAssignableFrom(sd.ImplementationType)).
               Select(sd => sd.ImplementationType!)];
    }
}
