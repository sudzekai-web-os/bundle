using Sudzekai.WebOs.Core;
using Sudzekai.WebOs.Plugin;
using TestPlugin.Context;

namespace TestPlugin.Handlers
{
    internal class FilteredErrorHandler : IEndpointHandler
    {
        public string Pattern => "error";

        public HandlerHttpMethod HttpMethod => HandlerHttpMethod.Get;

        public Task<HandlerResult> HandleAsync(HandlerRequest request)
        {
            return Task.FromResult(HandlerResult.BadRequest().WithData("string").AsJson(PluginJsonContext.Default.String));
        }

        public HandlerFilteringInfo? FilteringInfo { get; } = new(true);

        public HandlerOpenApiInfo? OpenApiInfo { get; } = new(
            "getError",
            "Get error",
            ["error"],
            "Возвращает ошибку")
        {
            Description = "Тестовый обработчик, возвращающий ошибку в обёртке ответа (filtered).",
            Responses =
            [
                new(400, PluginJsonContext.Default.String, "Ошибка")
            ]
        };
    }
}