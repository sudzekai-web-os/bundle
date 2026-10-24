using Microsoft.Extensions.Logging;
using Sudzekai.WebOs.Core;
using Sudzekai.WebOs.Plugin;

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

        public Task<HandlerResult> Handle(HandlerRequest request)
        {
            _logger.LogInformation(request.ToString());

            return Task.FromResult(HandlerResult.Ok().WithData("I am alive"));
        }
    }
}
