using Microsoft.Extensions.Logging;
using Sudzekai.WebOs.Core;
using Sudzekai.WebOs.Plugin;

namespace TestPlugin
{
    public class TestHandler : IEndpointHandler
    {
        private readonly ILogger<TestHandler> _logger;

        public TestHandler(ILogger<TestHandler> logger)
        {
            _logger = logger;
        }

        public string Pattern => "/health";

        public HandlerHttpMethod HttpMethod => HandlerHttpMethod.Get;

        public Task<HandlerResult> Handle(HandlerRequest request)
        {
            _logger.LogInformation("test handler handled");

            return Task.FromResult(HandlerResult.Ok().WithData("I am alive"));
        }
    }
}
