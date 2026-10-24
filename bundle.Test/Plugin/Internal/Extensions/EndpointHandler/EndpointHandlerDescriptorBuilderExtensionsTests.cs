using Sudzekai.WebOs.Core;
using Sudzekai.WebOs.Plugin;
using System.Net;

namespace bundle.Test.Plugin
{
    public class EndpointHandlerDescriptorBuilderExtensionsTests
    {
        [Fact]
        public unsafe void ToDescriptor_MapsAllHandlerMetadata()
        {
            var descriptor = new FullHandler().ToDescriptor();

            try
            {
                Assert.Equal(
                    "/api/items/{id}",
                    AllocationUtilities.ToString(descriptor.Pattern, descriptor.PatternLength));

                Assert.Equal((int)HandlerHttpMethod.Patch, descriptor.HttpMethod);

                Assert.Equal(1, descriptor.IsAuthorizedHandler);
                Assert.Equal(
                    (int)(ClientRole.Administrator | ClientRole.User | ClientRole.Viewer),
                    descriptor.RequiredRoles);

                Assert.Equal(1, descriptor.IsFilteredHandler);

                Assert.Equal(1, descriptor.IsLimitedHandler);
                Assert.Equal((int)RateLimitingPolicy.Strict, descriptor.RateLimitingPolicy);

                Assert.Equal(1, descriptor.IsOpenApiHandler);
                Assert.Equal(
                    "GetItem",
                    AllocationUtilities.ToString(descriptor.Name, descriptor.NameLength));
                Assert.Equal(
                    "Get item",
                    AllocationUtilities.ToString(descriptor.DisplayName, descriptor.DisplayNameLength));
                Assert.Equal(
                    new[] { "items", "read" },
                    AllocationUtilities.ToStringArray(descriptor.Tags, descriptor.TagsCount));
                Assert.Equal(
                    "Returns an item",
                    AllocationUtilities.ToString(descriptor.Summary, descriptor.SummaryLength));
                Assert.Equal(
                    "Items",
                    AllocationUtilities.ToString(descriptor.GroupName, descriptor.GroupNameLength));

                Assert.Equal(1, descriptor.IsRoutedHandler);
                Assert.Equal(7, descriptor.Priority);
                Assert.Equal(
                    new[] { "api.example.org", "www.example.org" },
                    AllocationUtilities.ToStringArray(descriptor.Hosts, descriptor.HostsCount));
            }
            finally
            {
                descriptor.Dispose();
            }
        }

        [Fact]
        public unsafe void ToDescriptor_MapsOnlyBaseFieldsForPlainHandler()
        {
            var descriptor = new PlainHandler().ToDescriptor();

            try
            {
                Assert.Equal(
                    "/health",
                    AllocationUtilities.ToString(descriptor.Pattern, descriptor.PatternLength));

                Assert.Equal((int)HandlerHttpMethod.Get, descriptor.HttpMethod);

                Assert.Equal(0, descriptor.IsAuthorizedHandler);
                Assert.Equal(0, descriptor.RequiredRoles);
                Assert.Equal(0, descriptor.IsFilteredHandler);
                Assert.Equal(0, descriptor.IsLimitedHandler);
                Assert.Equal(0, descriptor.IsOpenApiHandler);
                Assert.Equal(0, descriptor.IsRoutedHandler);
            }
            finally
            {
                descriptor.Dispose();
            }
        }

        [Fact]
        public unsafe void SetHandler_MapsHandlerFuncToCallablePointer()
        {
            EndpointHandlerDescriptor descriptor = default;

            descriptor.SetHandler(_ => Task.FromResult(
                HandlerResult.Create(HttpStatusCode.Created).WithData("created")));

            try
            {
                Assert.NotEqual(nint.Zero, descriptor.HandleFunc);

                var handle = (delegate* unmanaged<HandlerRequestDescriptor, HandlerResultDescriptor>)descriptor.HandleFunc;

                var resultDescriptor = handle(new HandlerRequestDescriptor
                {
                    HttpMethod = (int)HandlerHttpMethod.Get
                });

                try
                {
                    Assert.Equal((int)HttpStatusCode.Created, resultDescriptor.StatusCode);
                    Assert.Equal(
                        "created",
                        AllocationUtilities.ToString(resultDescriptor.Data, resultDescriptor.DataLength));
                }
                finally
                {
                    resultDescriptor.Dispose();
                }
            }
            finally
            {
                descriptor.Release();
            }
        }

        private sealed class FullHandler : IEndpointHandler
        {
            public string Pattern => "/api/items/{id}";

            public HandlerHttpMethod HttpMethod => HandlerHttpMethod.Patch;

            public HandlerAuthorizationInfo? AuthorizationInfo =>
                new([ClientRole.Administrator, ClientRole.User, ClientRole.Viewer]);

            public HandlerFilteringInfo? FilteringInfo => new(true);

            public HandlerLimitingInfo? LimitingInfo => new(RateLimitingPolicy.Strict);

            public HandlerOpenApiInfo? OpenApiInfo =>
                new("GetItem", "Get item", ["items", "read"], "Returns an item", "Items");

            public HandlerRoutingInfo? RoutingInfo =>
                new(7, ["api.example.org", "www.example.org"]);

            public Task<HandlerResult> HandleAsync(HandlerRequest request)
                => Task.FromResult(HandlerResult.Create(HttpStatusCode.OK));
        }

        private sealed class PlainHandler : IEndpointHandler
        {
            public string Pattern => "/health";

            public HandlerHttpMethod HttpMethod => HandlerHttpMethod.Get;

            public HandlerAuthorizationInfo? AuthorizationInfo => null;

            public HandlerFilteringInfo? FilteringInfo => null;

            public HandlerLimitingInfo? LimitingInfo => null;

            public HandlerOpenApiInfo? OpenApiInfo => null;

            public HandlerRoutingInfo? RoutingInfo => null;

            public Task<HandlerResult> HandleAsync(HandlerRequest request)
                => Task.FromResult(HandlerResult.Create(HttpStatusCode.OK));
        }
    }
}
