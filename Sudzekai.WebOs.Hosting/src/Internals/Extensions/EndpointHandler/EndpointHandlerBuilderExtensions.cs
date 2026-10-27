using Sudzekai.WebOs.Core;

namespace Sudzekai.WebOs.Hosting
{
    internal static class EndpointHandlerBuilderExtensions
    {
        extension(EndpointHandler)
        {
            public static EndpointHandler FromDescriptor(ref EndpointHandlerDescriptor descriptor)
            {
                EndpointHandler handler = CreateHandler(ref descriptor);

                handler.WithAuthorization(GetAuthorizationInfo(ref descriptor));
                handler.WithFilter(GetFilterInfo(ref descriptor));
                handler.WithLimiting(GetLimitingInfo(ref descriptor));
                handler.WithOpenApi(GetOpenApiInfo(ref descriptor));
                handler.WithRouting(GetRoutingInfo(ref descriptor));

                descriptor.Dispose();

                return handler;
            }
        }

        private static unsafe EndpointHandler CreateHandler(ref EndpointHandlerDescriptor descriptor)
        {
            var pattern = AllocationUtilities.ToString(descriptor.Pattern, descriptor.PatternLength);
            var method = (HandlerHttpMethod)descriptor.HttpMethod;
            var handleDelegate = (delegate* unmanaged<HandlerRequestDescriptor, HandlerResultDescriptor>)descriptor.HandleFunc;

            var handleFunc = (HandlerRequest req) =>
            {
                var describedResult = handleDelegate(req.ToDescriptor());

                return HandlerResult.FromDescriptor(describedResult);
            };

            return new EndpointHandler(pattern, method, handleFunc);
        }

        private static HandlerAuthorizationInfo? GetAuthorizationInfo(ref EndpointHandlerDescriptor descriptor)
        {
            if (descriptor.IsAuthorizedHandler == 0)
                return null;

            ClientRole[] requiredRoles = [];

            if ((descriptor.RequiredRoles & 1) != 0)
                requiredRoles = [.. requiredRoles.Append(ClientRole.Administrator)];

            if ((descriptor.RequiredRoles & 2) != 0)
                requiredRoles = [.. requiredRoles.Append(ClientRole.User)];

            if ((descriptor.RequiredRoles & 4) != 0)
                requiredRoles = [.. requiredRoles.Append(ClientRole.Viewer)];

            return new(requiredRoles);
        }

        private static HandlerFilteringInfo? GetFilterInfo(ref EndpointHandlerDescriptor descriptor)
            => new(descriptor.IsFilteredHandler != 0);

        private static HandlerLimitingInfo? GetLimitingInfo(ref EndpointHandlerDescriptor descriptor)
        {
            if (descriptor.IsLimitedHandler == 0)
                return null;

            var rateLimitingPolicy = (RateLimitingPolicy)descriptor.RateLimitingPolicy;

            if (!Enum.IsDefined(rateLimitingPolicy))
                throw new InvalidOperationException(LogsDictionary.EndpointHandlerBuilderExtensions.InvalidLimitingPolicy);

            return new(rateLimitingPolicy);
        }

        private static unsafe HandlerOpenApiInfo? GetOpenApiInfo(ref EndpointHandlerDescriptor descriptor)
        {
            if (descriptor.IsOpenApiHandler == 0)
                return null;

            var name = AllocationUtilities.ToString(descriptor.Name, descriptor.NameLength);
            var displayName = AllocationUtilities.ToString(descriptor.DisplayName, descriptor.DisplayNameLength);
            var tags = AllocationUtilities.ToStringArray(descriptor.Tags, descriptor.TagsCount);
            var summary = AllocationUtilities.ToString(descriptor.Summary, descriptor.SummaryLength);
            var groupName = AllocationUtilities.ToString(descriptor.GroupName, descriptor.GroupNameLength);

            return new(name, displayName, tags, summary, groupName);
        }

        private static unsafe HandlerRoutingInfo? GetRoutingInfo(ref EndpointHandlerDescriptor descriptor)
        {
            if (descriptor.IsRoutedHandler == 0)
                return null;

            return new(
                descriptor.Priority,
                AllocationUtilities.ToStringArray(descriptor.Hosts, descriptor.HostsCount)
            );
        }
    }
}
