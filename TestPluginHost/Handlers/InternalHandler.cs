using Sudzekai.WebOs.Core;

namespace TestPluginHost.Handlers
{
    public class InternalHandler : IEndpointHandler
    {
        public string Pattern => "/internal";

        public HandlerHttpMethod HttpMethod => HandlerHttpMethod.Get;

        public Task<HandlerResult> HandleAsync(HandlerRequest request)
        {
            return Task.FromResult(HandlerResult.Create(System.Net.HttpStatusCode.OK).WithData("Internal handler").As(ContentType.TextPlain));
        }
    }
}
