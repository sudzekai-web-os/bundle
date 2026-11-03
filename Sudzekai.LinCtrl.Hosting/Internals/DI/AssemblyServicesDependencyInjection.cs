using Microsoft.Extensions.DependencyInjection;

namespace Sudzekai.LinCtrl.Hosting 
{
    internal static class AssemblyServicesDependencyInjection
    {
        extension (IServiceCollection services)
        {
            public IServiceCollection AddAssemblyServices()
            {
                services.AddAssemblyLifetimeServices();

                services.AddAssemblyVTableValidator();

                services.AddSingleton<AssemblyLoader>();

                return services;
            }

            private IServiceCollection AddAssemblyVTableValidator()
            {
                services.AddSingleton<IAssemblyVTableFieldValidator, AssemblyVTableGetHttpRequestHandlersDelegateValidator>();

                services.AddSingleton<AssemblyVTableValidator>();

                return services;
            }
            
            private IServiceCollection AddAssemblyLifetimeServices()
            {
                services.AddSingleton<AssemblyDisposingHost>();
                services.AddSingleton<AssemblyInitializationHost>();

                return services;
            }
        }
    }
}
