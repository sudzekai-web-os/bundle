using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sudzekai.LinCtrl.Abi.Managed;
using System.Diagnostics.CodeAnalysis;
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

            public void AddHostRequestHandler<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler>(Action<EndpointBuilder> configure, int maxRequestBodyBytes)
                where THandler : class, IRequestHandler
            {
                var key = typeof(THandler);
                services.TryAddKeyedTransient<IRequestHandler, THandler>(key);
                services.AddSingleton(new HostRequestHandlerRegistration(
                    provider => provider.GetRequiredKeyedService<IRequestHandler>(key), configure, maxRequestBodyBytes));
            }

            private IServiceCollection AddBuildServices()
            {
                services.AddSingleton<RequestHandlerEndpointDataSource>();
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
