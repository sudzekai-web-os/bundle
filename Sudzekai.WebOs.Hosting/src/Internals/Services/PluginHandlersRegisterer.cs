using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sudzekai.WebOs.Core;
using System.Net;
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
            _logger.LogInformation(LogsDictionary.PluginHandlersRegisterer.PluginHandlersRegistrationStarted, plugin.Info.Name);

            int count = 0;

            var assemblyTag = $"{Path.GetFileName(Path.GetDirectoryName(plugin.AssemblyPath))}|{Path.GetFileName(plugin.AssemblyPath)}";

            foreach (var handler in plugin.Handlers)
            {

                RegisterHandler(handler, assemblyTag);

                count++;
            }

            _logger.LogInformation(LogsDictionary.PluginHandlersRegisterer.PluginHandlersRegistrationEnded, assemblyTag, count);
        }

        internal void RegisterHandler(IEndpointHandler handler, string assemblyTag)
        {
            var method = handler.HttpMethod.ToString().ToUpperInvariant();

            _logger.LogInformation(LogsDictionary.PluginHandlersRegisterer.HandlerRegistrationStarted, assemblyTag, method, handler.Pattern);

            var builder = CreateHandlerBuilder(handler);

            new HandlerConfigurator(builder, handler, assemblyTag, _provider).Configure();

            _logger.LogInformation(LogsDictionary.PluginHandlersRegisterer.HandlerRegistrationEnded, assemblyTag, method, handler.Pattern);
        }

        private IEndpointConventionBuilder CreateHandlerBuilder(IEndpointHandler handler)
            => _app.MapMethods(
                handler.Pattern,
                [handler.HttpMethod.ToString().ToUpperInvariant()],
                GetHandlerDelegate(handler));

        private RequestDelegate GetHandlerDelegate(IEndpointHandler handler)
            => async (HttpContext ctx) =>
            {
                var request = await HandlerRequest.FromHttpRequest(ctx.Request);

                var result = await handler.HandleAsync(request);

                ctx.Response.StatusCode = result.StatusCode;

                if (result.StatusCode == (int)HttpStatusCode.NoContent)
                    return;

                ctx.Response.ContentType = result.ContentType.ToBusinessString() + "; charset=utf-8";


                if (handler.FilteringInfo is not null && handler.FilteringInfo.IsEnabled)
                {
                    if (result.ContentType is ContentType.ApplicationJson)
                    {
                        await ReponseEnvelopeJsonWriter.WriteAsync(ctx, result);
                        return;
                    }

                    _logger.LogWarning(LogsDictionary.PluginHandlersRegisterer.HandlerUnsupportedContentType, 
                        handler.HttpMethod, handler.Pattern, result.ContentType.ToBusinessString());
                }

                await ctx.Response.WriteAsync(result.Data?.ToString() ?? "", Encoding.UTF8);
            };
    }
}
