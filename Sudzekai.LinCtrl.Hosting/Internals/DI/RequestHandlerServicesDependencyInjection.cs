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

                return services;
            }

            private IServiceCollection RegisterLoadingServices()
            {

                return services;
            }

            private IServiceCollection RegisterUnloadingServices()
            {
                return services;
            }
        }
    }
}
