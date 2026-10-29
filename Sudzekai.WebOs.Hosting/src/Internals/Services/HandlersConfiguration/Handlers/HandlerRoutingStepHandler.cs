using Microsoft.AspNetCore.Builder;
using Sudzekai.WebOs.Core;

namespace Sudzekai.WebOs.Hosting
{
    internal class HandlerRoutingStepHandler : IHandlerConfigurationStepHandler
    {
        public string Handle(IEndpointConventionBuilder builder, IEndpointHandler handler)
        {
            if (handler.RoutingInfo is not { } info)
                return LogsDictionary.HandlerRoutingStepHandler.RoutingSkipped;

            builder.WithOrder(-info.Priority);
            builder.RequireHost(info.Hosts);

            return LogsDictionary.HandlerRoutingStepHandler.RoutingAdded.Replace("{routingInfo}",
                $"Priority: {info.Priority}; " +
                $"Hosts: {string.Join(", ", info.Hosts)}");
        }
    }
}
