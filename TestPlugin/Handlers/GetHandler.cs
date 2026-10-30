using Microsoft.Extensions.Logging;
using Sudzekai.WebOs.Core;
using Sudzekai.WebOs.Plugin;
using TestPlugin.Context;

namespace TestPlugin.Handlers
{
    public class GetHandler : IEndpointHandler
    {
        private readonly ILogger<GetHandler> _logger;

        public GetHandler(ILogger<GetHandler> logger)
        {
            _logger = logger;
        }

        public string Pattern => "/health";

        public HandlerHttpMethod HttpMethod => HandlerHttpMethod.Get;

        public HandlerOpenApiInfo? OpenApiInfo { get; } = new(
            "health",
            "Health check",
            ["health"],
            "Проверка доступности сервиса")
        {
            Description = "Возвращает текстовую строку о том, что сервис работает.",
            Responses =
            [
                new(200, PluginJsonContext.Default.String, "Сервис работает", ContentType.TextPlain)
            ]
        };

        public Task<HandlerResult> HandleAsync(HandlerRequest request)
        {
            _logger.LogInformation(request.ToString());

            return Task.FromResult(HandlerResult.Ok().WithData("I am alive"));
        }
    }
}