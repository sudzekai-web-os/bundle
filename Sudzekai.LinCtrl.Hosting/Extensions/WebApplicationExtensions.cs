using Sudzekai.LinCtrl.Extensions;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Sudzekai.LinCtrl.Hosting.Internals.Services.Authentication.Services;

namespace Sudzekai.LinCtrl.Hosting
{
    public static class WebApplicationExtensions
    {
        extension(WebApplication application)
        {
            public void MapLinCtrlInfrastructure()
            {
                application.Services.GetRequiredService<RequestHandlerBuilderProvider>().SetRouteBuilder(application);

                if (application.Services.GetRequiredService<IServiceProviderIsService>().IsService(typeof(IJwtTokenService)))
                {
                    application.UseAuthentication();
                    application.UseMiddleware<JwtMiddleware>();
                    application.UseAuthorization();
                    application.UseRateLimiter();
                }

                var handlers = application.Services.GetRequiredService<RequestHandlerLifetimeHost>();
                using var scope = application.Services.CreateScope();
                var registrations = application.Services.GetServices<HostRequestHandlerRegistration>()
                    .Select(registration => (Registration: registration, Handler: registration.Resolve(scope.ServiceProvider)))
                    .ToArray();
                using var document = JsonDocument.Parse(registrations.Select(entry => entry.Handler).WriteOpenApiMetadata());
                var paths = document.RootElement.GetProperty("paths");
                foreach (var entry in registrations)
                    handlers.Build(entry.Handler, paths, entry.Registration);
            }

            public void LoadPlugins(string pluginsDirectoryPath = "plugins")
            {
                var pluginsLifetime = application.Services.GetRequiredService<PluginsLifetimeHost>();

                pluginsLifetime.Load(pluginsDirectoryPath);
            }

            public void InitializePlugins()
            {
                var pluginsLifetime = application.Services.GetRequiredService<PluginsLifetimeHost>();

                pluginsLifetime.Initialize(pluginsLifetime.GetLoadedPlugins());
            }

            public IReadOnlyList<PluginInfo> GetPlugins()
            {
                var pluginsLifetime = application.Services.GetRequiredService<PluginsLifetimeHost>();

                return pluginsLifetime.GetPlugins();
            }

            public void UnloadPlugins()
            {
                var pluginsLifetime = application.Services.GetRequiredService<PluginsLifetimeHost>();
                pluginsLifetime.Dispose(pluginsLifetime.GetPlugins());
            }
        }
    }
}
