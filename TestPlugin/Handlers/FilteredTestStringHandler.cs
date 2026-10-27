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
    }
}
