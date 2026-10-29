using Sudzekai.WebOs.Core;
using System.Runtime.InteropServices;

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

        public static EndpointHandlerDescriptor ToDescriptor(this IEndpointHandler handler)
        {
            EndpointHandlerDescriptor result = new()
            {
                Pattern = AllocationUtilities.AllocString(handler.Pattern, out var patternLength),
                PatternLength = patternLength,
                HttpMethod = (int)handler.HttpMethod
            };

            ConfigureAuthorizedHandler(ref result, handler.AuthorizationInfo);

            ConfigureFilteredHandler(ref result, handler.FilteringInfo);

            ConfigureLimitedHandler(ref result, handler.LimitingInfo);

            ConfigureRoutedHandler(ref result, handler.RoutingInfo);

            return result;
        }

        private static void ConfigureLimitedHandler(ref EndpointHandlerDescriptor descriptor, HandlerLimitingInfo? info)
        {
            if (info is null)
                return;

            descriptor.IsLimitedHandler = 1;
            descriptor.RateLimitingPolicy = (int)info.RateLimitingPolicy;
        }

        private static void ConfigureAuthorizedHandler(ref EndpointHandlerDescriptor descriptor, HandlerAuthorizationInfo? info)
        {
            if (info is null)
                return;

            descriptor.IsAuthorizedHandler = 1;
            foreach (var role in info.RequiredRoles)
                descriptor.RequiredRoles |= (int)role;
        }

        private static void ConfigureFilteredHandler(ref EndpointHandlerDescriptor descriptor, HandlerFilteringInfo? info)
        {
            if (info is null)
                return;

            descriptor.IsFilteredHandler = 1;
        }

        private static unsafe void ConfigureRoutedHandler(ref EndpointHandlerDescriptor descriptor, HandlerRoutingInfo? info)
        {
            if (info is null)
                return;

            descriptor.IsRoutedHandler = 1;
            descriptor.Priority = info.Priority;
            descriptor.Hosts = AllocationUtilities.AllocStringArray(info.Hosts, out var count);
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
