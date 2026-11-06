using Microsoft.Extensions.DependencyInjection;
using Sudzekai.LinCtrl.Hosting.Internals.Services.Handlers.Services;

namespace Sudzekai.LinCtrl.Hosting
{
    internal static class RequestHandlerServicesDependencyInjection
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection RegisterRequestHandlerServices()
            {
                services.AddResponseWriters();
                services.AddBuildServices();
                services.AddRegisterServices();

                services.AddSingleton<RequestHandlerLifetimeHost>();

                return services;
            }

            private IServiceCollection AddBuildServices()
            {
                services.AddSingleton<RequestHandlerBuilderProvider>();
                services.AddSingleton<IRequestHandlerBuilder>(sp => sp.GetRequiredService<RequestHandlerBuilderProvider>());
                services.AddSingleton<IRequestHandlerBuilder, RequestHandlerDelegateBuilder>();

                return services;
            }

            private IServiceCollection AddResponseWriters()
            {
                services.AddSingleton<IResponseWriter, JsonResponseWriter>();

                return services;
            }

            private IServiceCollection AddRegisterServices()
            {
                return services;
            }
        }
    }
}
