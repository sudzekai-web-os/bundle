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

        public PluginHandlersRegisterer(ILogger<PluginHandlersRegisterer> logger)
        {
            _logger = logger;
        }

        public void RegisterHandlers(WebApplication app, Plugin plugin)
        {
            _logger.LogInformation("[{assemblyTag}] Регистрация обработчиков...", plugin.Info.Name);

            int count = 0;

            var assemblyTag = $"{Path.GetFileName(Path.GetDirectoryName(plugin.AssemblyPath))}|{Path.GetFileName(plugin.AssemblyPath)}";

            foreach (var handler in plugin.Handlers)
            {
                _logger.LogInformation("[{assemblyTag}] Регистрация обработчика {method} -> {pattern}", assemblyTag, handler.HttpMethod.ToString(),  handler.Pattern);
                var builder = CreateHandlerBuilder(app, handler);
                _logger.LogDebug("[{assemblyTag}] (1/5) Создан конструктор обработчика", assemblyTag);

                var roles = AddAuthorization(builder, handler);
                if (roles.Length == 0)
                    _logger.LogDebug("[{assemblyTag}] (2/5) Авторизация не требуется. Пункт пропускается", assemblyTag);
                else
                    _logger.LogDebug("[{assemblyTag}] (2/5) Добавлена авторизация. Роли: {roles}", assemblyTag, string.Join(", ", roles));

                var policy = AddLimiting(builder, handler);
                _logger.LogDebug("[{assemblyTag}] (3/5) Добавлена политика ограничения. Политика: {policy}", assemblyTag, policy);

                var openApiInfo = AddOpenApi(builder, handler);
                if (openApiInfo.Count == 0)
                    _logger.LogDebug("[{assemblyTag}] (4/5) Конфигурация OpenApi не требуется. Пункт пропускается", assemblyTag);
                else
                    _logger.LogDebug("[{assemblyTag}] (4/5) Добавлена конфигурация OpenApi. Параметры: {openApiInfo}",
                         handler.Pattern, string.Join(", ", openApiInfo.Select(kv => $"{kv.Key}: {kv.Value}")));

                var routingInfo = AddRouting(builder, handler);
                if (routingInfo.Count == 0)
                    _logger.LogDebug("[{assemblyTag}] (5/5) Конфигурация роутинга не требуется. Пункт пропускается", assemblyTag);
                else
                    _logger.LogDebug("[{assemblyTag}] (5/5) Добавлена конфигурация роутинга. Параметры: {routingInfo}",
                         handler.Pattern, string.Join(", ", routingInfo.Select(kv => $"{kv.Key}: {kv.Value}")));

                _logger.LogInformation("[{assemblyTag}] Обработчик {method} -> {pattern} зарегистрирован", assemblyTag, handler.HttpMethod.ToString(), handler.Pattern);

                count++;
            }

            _logger.LogInformation("[{assemblyTag}] Зарегистрировано {count} обработчиков", assemblyTag, count);
        }

        private static RouteHandlerBuilder CreateHandlerBuilder(WebApplication app, EndpointHandler handler)
            => handler.HttpMethod switch
            {
                HandlerHttpMethod.Get => app.MapGet(handler.Pattern, GetHandlerDelegate(app, handler)),
                HandlerHttpMethod.Put => app.MapPut(handler.Pattern, GetHandlerDelegate(app, handler)),
                HandlerHttpMethod.Post => app.MapPut(handler.Pattern, GetHandlerDelegate(app, handler)),
                HandlerHttpMethod.Patch => app.MapPut(handler.Pattern, GetHandlerDelegate(app, handler)),
                HandlerHttpMethod.Delete => app.MapPut(handler.Pattern, GetHandlerDelegate(app, handler)),
                _ => throw new NotSupportedException("http метод не поддерживается"),
            };

        private static string[] AddAuthorization(RouteHandlerBuilder builder, EndpointHandler handler)
        {
            if (!handler.IsAuthorizedHandler)
                return [];

            string[] roles = [.. handler.RequiredRoles.Select(r => r.ToString())];

            builder.RequireAuthorization(policy =>
            {
                foreach (var role in roles)
                    policy.RequireRole(role.ToString());
            });

            return roles;
        }

        private static string AddLimiting(RouteHandlerBuilder builder, EndpointHandler handler)
        {
            var policy = handler.IsLimitedHandler
                ? handler.RateLimitingPolicy.ToString() : RateLimitingPolicy.Default.ToString();

            builder.RequireRateLimiting(policy);

            return policy;
        }

        private static Dictionary<string, string> AddOpenApi(RouteHandlerBuilder builder, EndpointHandler handler)
        {
            if (!handler.IsOpenApiHandler)
                return [];

            Dictionary<string, string> result = [];

            result["Name"] = handler.Name;
            builder.WithName(handler.Name);

            result["DisplayName"] = handler.DisplayName;
            builder.WithDisplayName(handler.DisplayName);

            result["Summary"] = handler.Summary;
            builder.WithSummary(handler.Summary);

            result["Tags"] = string.Join(", ", handler.Tags);
            builder.WithTags(handler.Tags);

            result["GroupName"] = handler.GroupName;
            builder.WithGroupName(handler.GroupName);

            return result;
        }

        private static Dictionary<string, string> AddRouting(RouteHandlerBuilder builder, EndpointHandler handler)
        {
            if (!handler.IsRoutedHandler)
                return [];

            Dictionary<string, string> result = [];

            result["Priority"] = handler.Priority.ToString();
            builder.WithOrder(-handler.Priority);

            result["Hosts"] = string.Join(",", handler.Hosts);
            builder.RequireHost(handler.Hosts);

            return result;
        }

        private static Delegate GetHandlerDelegate(WebApplication app, EndpointHandler handler)
            => async (HttpContext ctx) =>
            {
                var logger = app.Services.GetRequiredService<ILogger<EndpointHandler>>();

                var request = await HandlerRequest.FromHttpRequest(ctx.Request);

                logger.LogDebug("Получен {HttpMethod} запрос на {Path}", request.HttpMethod.ToString(), request.Path);

                var result = await handler.HandleAsync(request);

                ctx.Response.StatusCode = result.StatusCode;
                ctx.Response.ContentType = result.ContentType.ToBusinessString();
                await ctx.Response.WriteAsync(result.Data?.ToString() ?? "", Encoding.UTF8);
            };
    }
}
