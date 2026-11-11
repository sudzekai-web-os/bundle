using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;

namespace Sudzekai.LinCtrl.Hosting
{
    public static class WebApplicationExtensions
    {
        extension(WebApplication application)
        {
            public void MapLinCtrlInfrastructure()
            {
                application.Services.GetRequiredService<RequestHandlerBuilderProvider>().SetRouteBuilder(application);

                if (!application.Services.GetRequiredService<IServiceProviderIsService>().IsService(typeof(LoginHandler)))
                    return;

                application.UseAuthentication();
                application.UseAuthorization();
                application.UseRateLimiter();

                application.MapPost(LoginHandler.Pattern, (Func<HttpContext, Task>)(context =>
                    context.RequestServices.GetRequiredService<LoginHandler>().HandleAsync(context)))
                    .WithName("LinCtrl.Login")
                    .WithSummary("Sign in")
                    .AllowAnonymous()
                    .RequireRateLimiting(AuthenticationServicesDependencyInjection.LoginRateLimit)
                    .WithMetadata(
                        new AcceptsMetadata(["application/json"], typeof(LoginCommand)),
                        new ProducesResponseTypeMetadata(200, typeof(LoginToken), ["application/json"]),
                        new ProducesResponseTypeMetadata(400, typeof(void), []),
                        new ProducesResponseTypeMetadata(401, typeof(void), []),
                        new ProducesResponseTypeMetadata(413, typeof(void), []),
                        new ProducesResponseTypeMetadata(415, typeof(void), []),
                        new ProducesResponseTypeMetadata(429, typeof(void), []));
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
