using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sudzekai.WebOs.Core;

namespace Sudzekai.WebOs.Hosting
{
    internal class HandlerConfigurator
    {
        private readonly ILogger<HandlerConfigurator> _logger;
        private readonly IEndpointHandler _handler;
        private readonly RouteHandlerBuilder _builder;
        private readonly string _assemblyTag;

        public HandlerConfigurator(
            RouteHandlerBuilder builder,
            IEndpointHandler handler,
            string assemblyTag,
            IServiceProvider sp)
        {
            _builder = builder;
            _handler = handler;
            _assemblyTag = assemblyTag;
            _logger = sp.GetRequiredService<ILogger<HandlerConfigurator>>();
        }

        public void Configure()
        {
            var roles = AddAuthorization();
            if (roles.Length == 0)
                _logger.LogDebug("[{assemblyTag}] (1/4) Авторизация не требуется. Пункт пропускается", _assemblyTag);
            else
                _logger.LogDebug("[{assemblyTag}] (1/4) Добавлена авторизация. Роли: {roles}", _assemblyTag, string.Join(", ", roles));

            var policy = AddLimiting();
            _logger.LogDebug("[{assemblyTag}] (2/4) Добавлена политика ограничения. Политика: {policy}", _assemblyTag, policy);

            var openApiInfo = AddOpenApi();
            if (openApiInfo.Count == 0)
                _logger.LogDebug("[{assemblyTag}] (3/4) Конфигурация OpenApi не требуется. Пункт пропускается", _assemblyTag);
            else
                _logger.LogDebug("[{assemblyTag}] (3/4) Добавлена конфигурация OpenApi. Параметры: {openApiInfo}", _assemblyTag, string.Join(", ", openApiInfo.Select(kv => $"{kv.Key}: {kv.Value}")));

            var routingInfo = AddRouting();
            if (routingInfo.Count == 0)
                _logger.LogDebug("[{assemblyTag}] (4/4) Конфигурация роутинга не требуется. Пункт пропускается", _assemblyTag);
            else
                _logger.LogDebug("[{assemblyTag}] (4/4) Добавлена конфигурация роутинга. Параметры: {routingInfo}", _assemblyTag, string.Join(", ", routingInfo.Select(kv => $"{kv.Key}: {kv.Value}")));
        }

        private string[] AddAuthorization()
        {
            if (_handler.AuthorizationInfo is not { } info)
                return [];

            string[] roles = [.. info.RequiredRoles.Select(r => r.ToString())];

            _builder.RequireAuthorization(policy =>
            {
                foreach (var role in roles)
                    policy.RequireRole(role);
            });

            return roles;
        }

        private string AddLimiting()
        {
            var policy = RateLimitingPolicy.Default.ToString();

            if (_handler.LimitingInfo is { } info)
                policy = info.RateLimitingPolicy.ToString();

            _builder.RequireRateLimiting(policy);

            return policy;
        }

        private Dictionary<string, string> AddOpenApi()
        {
            if (_handler.OpenApiInfo is not { } info)
                return [];

            Dictionary<string, string> result = [];

            result["Name"] = info.Name;
            _builder.WithName(info.Name);

            result["DisplayName"] = info.DisplayName;
            _builder.WithDisplayName(info.DisplayName);

            result["Summary"] = info.Summary;
            _builder.WithSummary(info.Summary);

            result["Tags"] = string.Join(", ", info.Tags);
            _builder.WithTags(info.Tags);

            result["GroupName"] = info.GroupName;
            _builder.WithGroupName(info.GroupName);

            return result;
        }

        private Dictionary<string, string> AddRouting()
        {
            if (_handler.RoutingInfo is not { } info)
                return [];

            Dictionary<string, string> result = [];

            result["Priority"] = info.Priority.ToString();
            _builder.WithOrder(-info.Priority);

            result["Hosts"] = string.Join(",", info.Hosts);
            _builder.RequireHost(info.Hosts);

            return result;
        }
    }
}