using Microsoft.Extensions.Logging;
using Sudzekai.WebOs.Core;
using Sudzekai.WebOs.Plugin;
using TestPlugin.Context;

namespace TestPlugin.Handlers
{
    public class QueriedHandler : IEndpointHandler
    {
        private readonly ILogger<GetHandler> _logger;

        public QueriedHandler(ILogger<GetHandler> logger)
        {
            _logger = logger;
        }

        public string Pattern => "/query/{id}";

        public HandlerHttpMethod HttpMethod => HandlerHttpMethod.Get;

        public HandlerOpenApiInfo? OpenApiInfo { get; } = new(
            "getQueryById",
            "Get query by id",
            ["query"],
            "Возвращает значение маршрутного параметра")
        {
            Description = "Читает маршрутный параметр id и возвращает его значение.",
            Responses =
            [
                new(200, PluginJsonContext.Default.String, "Значение маршрутного параметра"),
                new(400, PluginJsonContext.Default.String, "Некорректный идентификатор")
            ]
        };

        public Task<HandlerResult> HandleAsync(HandlerRequest request)
        {
            _logger.LogInformation(request.ToString());

            return Task.FromResult(HandlerResult.Ok().WithData(request.RouteValues["id"]).AsJson(PluginJsonContext.Default.String));
        }
    }
}