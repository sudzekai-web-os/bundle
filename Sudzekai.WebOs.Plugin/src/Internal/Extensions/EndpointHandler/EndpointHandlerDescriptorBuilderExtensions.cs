using Sudzekai.WebOs.Core;
using System.Runtime.InteropServices;
using System.Text;

namespace Sudzekai.WebOs.Plugin
{
    internal static class EndpointHandlerDescriptorBuilderExtensions
    {
        private delegate HandlerResultDescriptor DescribedHandleFunc(HandlerRequestDescriptor requestDescriptor);

        public static void SetHandler(ref this EndpointHandlerDescriptor descriptor, Func<HandlerRequest, Task<HandlerResult>> handleFunc)
        {
            var describedHandleFunc = GetDescribedHandleFunc(handleFunc);

            descriptor.HandleFunc = Marshal.GetFunctionPointerForDelegate(describedHandleFunc);

            EndpointHandlerCallbacksContainer.Fix(descriptor.HandleFunc, describedHandleFunc);
        }

        public static unsafe EndpointHandlerDescriptor ToDescriptor(this IEndpointHandler handler)
        {
            EndpointHandlerDescriptor result = new()
            {
                Pattern = AllocationUtilities.AllocString(handler.Pattern, out var patternLength),
                PatternLength = patternLength,
                HttpMethod = (int)handler.HttpMethod
            };

            if (handler is IAuthorizedEndpointHandler authorizedHandler)
                ConfigureAuthorizedHandler(ref result, authorizedHandler);

            if (handler is IFilteredEndpointHandler filteredHandler)
                ConfigureFilteredHandler(ref result, filteredHandler);

            if (handler is ILimitedEndpointHandler limitedHandler)
                ConfigureLimitedHandler(ref result, limitedHandler);

            if (handler is IOpenApiEndpointHandler openApiHandler)
                ConfigureOpenApiHandler(ref result, openApiHandler);

            if (handler is IRoutedEndpointHandler routedHandler)
                ConfigureRoutedHandler(ref result, routedHandler);

            return result;
        }

        private static unsafe void ConfigureOpenApiHandler(ref EndpointHandlerDescriptor descriptor, IOpenApiEndpointHandler openApiHandler)
        {
            descriptor.IsOpenApiHandler = 1;
            descriptor.Name = AllocationUtilities.AllocString(openApiHandler.Name, out var nameLength);
            descriptor.NameLength = nameLength;

            descriptor.DisplayName = AllocationUtilities.AllocString(openApiHandler.DisplayName, out var displayNameLength);
            descriptor.DisplayNameLength = displayNameLength;

            descriptor.Tags = AllocationUtilities.AllocStringArray(openApiHandler.Tags, out var count);
            descriptor.TagsCount = count;

            descriptor.Summary = AllocationUtilities.AllocString(openApiHandler.Summary, out var summaryLength);
            descriptor.SummaryLength = summaryLength;

            descriptor.GroupName = AllocationUtilities.AllocString(openApiHandler.GroupName, out var groupNameLength);
            descriptor.GroupNameLength = groupNameLength;
        }

        private static void ConfigureLimitedHandler(ref EndpointHandlerDescriptor descriptor, ILimitedEndpointHandler limitedHandler)
        {
            descriptor.IsLimitedHandler = 1;
            descriptor.RateLimitingPolicy = (int)limitedHandler.RateLimitingPolicy;
        }

        private static void ConfigureAuthorizedHandler(ref EndpointHandlerDescriptor descriptor, IAuthorizedEndpointHandler authorizedHandler)
        {
            descriptor.IsAuthorizedHandler = 1;
            foreach (var role in authorizedHandler.RequiredRoles)
                descriptor.RequiredRoles |= (int)role;
        }

        private static void ConfigureFilteredHandler(ref EndpointHandlerDescriptor descriptor, IFilteredEndpointHandler filteredHandler)
        {
            descriptor.IsFilteredHandler = 1;
        }

        private static unsafe void ConfigureRoutedHandler(ref EndpointHandlerDescriptor descriptor, IRoutedEndpointHandler routedHandler)
        {
            descriptor.IsRoutedHandler = 1;
            descriptor.Priority = routedHandler.Priority;
            descriptor.Hosts = AllocationUtilities.AllocStringArray(routedHandler.Hosts, out var count);
            descriptor.HostsCount = count;
        }

        private static DescribedHandleFunc GetDescribedHandleFunc(Func<HandlerRequest, Task<HandlerResult>> handleFunc)
            => (requestDescriptor) =>
            {
                var request = HandlerRequest.FromDescriptor(requestDescriptor);

                var result = handleFunc(request).GetAwaiter().GetResult();

                return result.ToDescriptor();
            };
    }
}
