using Microsoft.Extensions.DependencyInjection;
using Sudzekai.LinCtrl.Hosting.Internals.Services.Assembly.Initialization;

namespace Sudzekai.LinCtrl.Hosting
{
    internal static class AssemblyServicesDependencyInjection
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddAssemblyServices()
            {
                services.AddLoadingServices();

                services.AddInitializationServices();

                services.AddSingleton<AssemblyLifetimeHost>();

                return services;
            }

            private IServiceCollection AddLoadingServices()
            {
                services.AddSingleton<IAssemblyLoader, AssemblyVTableLoader>();

                return services;
            }

            private IServiceCollection AddInitializationServices()
            {
                services.AddSingleton<IAssemblyInitializer, AssemblyInitializer>();
                services.AddSingleton<IAssemblyInitializer, AssemblyLoggingDelegator>();
                services.AddSingleton<IAssemblyInitializer, AssemblyBuilder>();
                services.AddSingleton<IAssemblyInitializer, AssemblyRequestHandlersLoader>();
                services.AddSingleton<IAssemblyInitializer, AssemblyRequestHandlersConfigurator>();
                services.AddSingleton<IAssemblyInitializer, AssemblyInitializationFinalizer>();

                return services;
            }
        }
    }
}
