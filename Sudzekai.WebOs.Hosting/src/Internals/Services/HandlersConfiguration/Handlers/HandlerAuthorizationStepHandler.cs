using Microsoft.AspNetCore.Builder;
using Sudzekai.WebOs.Core;

namespace Sudzekai.WebOs.Hosting
{
    internal class HandlerAuthorizationStepHandler : IHandlerConfigurationStepHandler
    {
        public string Handle(IEndpointConventionBuilder builder, IEndpointHandler handler)
        {
            if (handler.AuthorizationInfo is not { } info)
                return LogsDictionary.HandlerAuthorizationStepHandler.AuthorizationSkipped;

            string[] roles = [.. info.RequiredRoles.Select(r => r.ToString())];

            builder.RequireAuthorization(policy =>
            {
                foreach (var role in roles)
                    policy.RequireRole(role);
            });

            return LogsDictionary.HandlerAuthorizationStepHandler.AuthorizationAdded.Replace("{roles}", string.Join(", ", roles));
        }
    }
}
