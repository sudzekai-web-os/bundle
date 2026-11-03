using Microsoft.Extensions.DependencyInjection;

namespace Sudzekai.LinCtrl.Hosting 
{
    internal static class RequestHandlerServicesDependencyInjection
    {
        extension (IServiceCollection services)
        {
            public IServiceCollection RegisterRequestHandlerServices()
            {
                services.RegisterLoadingServices();
                services.RegisterUnloadingServices();
                
                services.RegisterLifeTimeServices();

                return services;
            }

            private IServiceCollection RegisterLifeTimeServices()
            {
                services.AddSingleton<PluginRequestHandlersDisposingHost>();
                services.AddSingleton<PluginRequestHandlersInitializationHost>();

                services.AddSingleton<PluginRequestHandlersLifetimeHost>();

                return services;
            }

            private IServiceCollection RegisterLoadingServices()
            {
                services.AddSingleton<RequestHandlerLoader>();
                services.AddSingleton<RequestHandlerLoadingHost>();

                return services;
            }

            private IServiceCollection RegisterUnloadingServices()
            {
                services.AddSingleton<RequestHandlerUnloader>();
                services.AddSingleton<RequestHandlerUnloadingHost>();

                return services;
            }
        }
    }
}
