using Microsoft.Extensions.Logging;
using Sudzekai.WebOs.Core;
using Sudzekai.WebOs.Plugin;

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

        public Task<HandlerResult> Handle(HandlerRequest request)
        {
            _logger.LogInformation(request.ToString());

            return Task.FromResult(HandlerResult.Ok().WithData(request.RouteValues["id"]));
        }
    }
}
