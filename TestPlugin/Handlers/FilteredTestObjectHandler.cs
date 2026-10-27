using Sudzekai.WebOs.Core;
using Sudzekai.WebOs.Plugin;
using System;
using System.Collections.Generic;
using System.Text;
using TestPlugin.Context;
using TestPlugin.Object;

namespace TestPlugin.Handlers
{
    internal class FilteredTestObjectHandler : IEndpointHandler
    {
        public string Pattern => "object";

        public HandlerHttpMethod HttpMethod => HandlerHttpMethod.Get;

        public Task<HandlerResult> HandleAsync(HandlerRequest request)
        {
            return Task.FromResult(HandlerResult.Ok().WithData(new TestObject()).AsJson(PluginJsonContext.Default.TestObject));
        }

        public HandlerFilteringInfo FilteringInfo { get; } = new(true);
    }
}
