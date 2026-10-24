using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sudzekai.WebOs.Core;
using System.Text;

namespace Sudzekai.WebOs.Hosting
{
    internal class PluginHandlersRegisterer
    {
        private readonly ILogger<PluginHandlersRegisterer> _logger;
        private readonly IServiceProvider _provider;
        private WebApplication _app;

        public PluginHandlersRegisterer(WebApplication app)
        {
            _app = app;
            _provider = app.Services;
            _logger = _provider.GetRequiredService<ILogger<PluginHandlersRegisterer>>();
        }

        public void RegisterHandlers(Plugin plugin)
        {
            _logger.LogInformation("[{assemblyTag}] Регистрация обработчиков плагина...", plugin.Info.Name);

            int count = 0;

            var assemblyTag = $"{Path.GetFileName(Path.GetDirectoryName(plugin.AssemblyPath))}|{Path.GetFileName(plugin.AssemblyPath)}";

            foreach (var handler in plugin.Handlers)
            {

                RegisterHandler(handler, assemblyTag);

                count++;
            }

            _logger.LogInformation("[{assemblyTag}] Зарегистрировано {count} обработчиков плагина", assemblyTag, count);
        }

        internal void RegisterHandler(IEndpointHandler handler, string assemblyTag)
        {
            var method = handler.HttpMethod.ToString().ToUpperInvariant();

            _logger.LogInformation("[{assemblyTag}] Регистрация обработчика {method} -> {pattern}...", assemblyTag, method, handler.Pattern);

            var builder = CreateHandlerBuilder(handler);

            new HandlerConfigurator(builder, handler, assemblyTag, _provider).Configure();

            _logger.LogInformation("[{assemblyTag}] Обработчик {method} -> {pattern} зарегистрирован", assemblyTag, method, handler.Pattern);
        }

        private RouteHandlerBuilder CreateHandlerBuilder(IEndpointHandler handler)
            => _app.MapMethods(
                handler.Pattern,
                [handler.HttpMethod.ToString().ToUpperInvariant()],
                GetHandlerDelegate(handler));

        private static Delegate GetHandlerDelegate(IEndpointHandler handler)
            => async (HttpContext ctx) =>
            {
                var request = await HandlerRequest.FromHttpRequest(ctx.Request);

                var result = await handler.HandleAsync(request);

                ctx.Response.StatusCode = result.StatusCode;
                ctx.Response.ContentType = result.ContentType.ToBusinessString();
                await ctx.Response.WriteAsync(result.Data?.ToString() ?? "", Encoding.UTF8);
            };
    }
}
