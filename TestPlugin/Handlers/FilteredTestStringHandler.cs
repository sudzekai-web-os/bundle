using Sudzekai.WebOs.Core;
using Sudzekai.WebOs.Plugin;
using TestPlugin.Context;

namespace TestPlugin.Handlers
{
    internal class FilteredTestStringHandler : IEndpointHandler
    {
        public string Pattern => "string";

        public HandlerHttpMethod HttpMethod => HandlerHttpMethod.Get;

        public Task<HandlerResult> HandleAsync(HandlerRequest request)
        {
            return Task.FromResult(HandlerResult.Ok().WithData("string").AsJson(PluginJsonContext.Default.String));
        }

        public HandlerFilteringInfo FilteringInfo { get; } = new(true);

        public HandlerOpenApiInfo? OpenApiInfo { get; } = new(
            "getTestString",
            "Get test string",
            ["string"],
            "Возвращает тестовую строку")
        {
            Description = "Возвращает строку в обёртке ответа (filtered).",
            Responses =
            [
                new(200, PluginJsonContext.Default.String)
            ]
        };
    }
}