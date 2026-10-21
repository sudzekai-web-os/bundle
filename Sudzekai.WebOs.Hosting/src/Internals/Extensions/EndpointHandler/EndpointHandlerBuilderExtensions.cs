using Sudzekai.WebOs.Core;

namespace Sudzekai.WebOs.Hosting
{
    internal static class EndpointHandlerBuilderExtensions
    {
        extension(EndpointHandler)
        {
            public static EndpointHandler FromDescriptor(ref EndpointHandlerDescriptor descriptor)
            {
                EndpointHandler handler = new();

                handler.AsDefaultHandler(ref descriptor);
                handler.AsAuthorizedHandler(in descriptor);
                handler.AsFilteredHandler(in descriptor);
                handler.AsLimitedHandler(in descriptor);
                handler.AsOpenApiHandler(descriptor);
                handler.AsRoutedHandler(descriptor);

                descriptor.Dispose();

                return handler;
            }
        }

        private static unsafe void AsDefaultHandler(this EndpointHandler handler, ref EndpointHandlerDescriptor descriptor)
        {
            string pattern;

            fixed (byte* p = descriptor.Pattern)
                pattern = AllocationUtilities.PatternToString(p);

            var method = (HandlerHttpMethod)descriptor.HttpMethod;

            if (!Enum.IsDefined(method))
                throw new InvalidOperationException("дескриптор содержал невалидный http метод");

            var handleDelegate = (delegate* unmanaged<HandlerRequestDescriptor, HandlerResultDescriptor>)descriptor.HandleFunc;

            Func<HandlerRequest, HandlerResult> handleFunc = (req) =>
            {
                var describedResult = handleDelegate(req.ToDescriptor());

                return HandlerResult.FromDescriptor(describedResult);
            };

            handler.Pattern = pattern;
            handler.HttpMethod = method;
            handler.HandleFunc = handleFunc;
        }

        private static void AsAuthorizedHandler(this EndpointHandler handler, in EndpointHandlerDescriptor descriptor)
        {
            handler.IsAuthorizedHandler = descriptor.IsAuthorizedHandler > 0;

            if (!handler.IsAuthorizedHandler)
                return;

            ClientRole[] requiredRoles = [];

            if ((descriptor.RequiredRoles & 1) != 0)
                requiredRoles = [.. requiredRoles.Append(ClientRole.Administrator)];

            if ((descriptor.RequiredRoles & 2) != 0)
                requiredRoles = [.. requiredRoles.Append(ClientRole.User)];

            if ((descriptor.RequiredRoles & 4) != 0)
                requiredRoles = [.. requiredRoles.Append(ClientRole.Viewer)];

            handler.RequiredRoles = requiredRoles;
        }

        private static void AsFilteredHandler(this EndpointHandler handler, in EndpointHandlerDescriptor descriptor)
        {
            handler.IsFilteredHandler = descriptor.IsFilteredHandler > 0;
        }

        private static void AsLimitedHandler(this EndpointHandler handler, in EndpointHandlerDescriptor descriptor)
        {
            handler.IsLimitedHandler = descriptor.IsLimitedHandler > 0;

            if (!handler.IsLimitedHandler)
                return;

            var rateLimitingPolicy = (RateLimitingPolicy)descriptor.RateLimitingPolicy;

            if (!Enum.IsDefined(rateLimitingPolicy))
                throw new InvalidOperationException("дескриптор содержал невалидную политику ограничения");

            handler.RateLimitingPolicy = rateLimitingPolicy;
        }

        private static unsafe void AsOpenApiHandler(this EndpointHandler handler, in EndpointHandlerDescriptor descriptor)
        {
            handler.IsOpenApiHandler = descriptor.IsOpenApiHandler > 0;

            if (!handler.IsOpenApiHandler)
                return;

            handler.Name = AllocationUtilities.ToString(descriptor.Name, descriptor.NameLength);
            handler.DisplayName = AllocationUtilities.ToString(descriptor.DisplayName, descriptor.DisplayNameLength);
            handler.Tags = AllocationUtilities.ToStringArray(descriptor.Tags, descriptor.TagsCount);
            handler.Summary = AllocationUtilities.ToString(descriptor.Summary, descriptor.SummaryLength);
            handler.GroupName = AllocationUtilities.ToString(descriptor.GroupName, descriptor.GroupNameLength);
        }

        private static unsafe void AsRoutedHandler(this EndpointHandler handler, in EndpointHandlerDescriptor descriptor)
        {
            handler.IsRoutedHandler = descriptor.IsRoutedHandler > 0;

            if (!handler.IsRoutedHandler)
                return;

            handler.Priority = descriptor.Priority;

            handler.Hosts = AllocationUtilities.ToStringArray(descriptor.Hosts, descriptor.HostsCount);
        }
    }
}
