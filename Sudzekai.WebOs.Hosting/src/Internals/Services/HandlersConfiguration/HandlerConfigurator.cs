using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sudzekai.WebOs.Core;

namespace Sudzekai.WebOs.Hosting
{
    internal class HandlerConfigurator(IServiceProvider sp)
    {
        public void Configure(IEndpointConventionBuilder builder, IEndpointHandler handler, string assemblyTag)
        {
            var logger = sp.GetRequiredService<ILogger<HandlerConfigurator>>();

            var stepHandlers = sp.GetServices<IHandlerConfigurationStepHandler>();

            var step = 0;
            var stepHandlersCount = stepHandlers.Count();

            foreach (var stepHandler in stepHandlers)
            {
                step++;

                var resultMessage = stepHandler.Handle(builder, handler);

                logger.LogDebug(resultMessage, assemblyTag, $"{step}/{stepHandlersCount}");
            }
        }
    }
}