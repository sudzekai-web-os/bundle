using Sudzekai.WebOs.Core;
using Sudzekai.WebOs.Hosting;
using Sudzekai.WebOs.Plugin;
using System.Net;

namespace bundle.Test.Hosting
{
    public class EndpointHandlerBuilderExtensionsTests
    {
        [Fact]
        public async Task FromDescriptor_MapsBaseHandlerAndHandleFunc()
        {
            var descriptor = CreateBaseDescriptor();

            EndpointHandler handler;

            try
            {
                handler = EndpointHandler.FromDescriptor(ref descriptor);

                Assert.Equal("/health", handler.Pattern);
                Assert.Equal(HandlerHttpMethod.Get, handler.HttpMethod);
                Assert.NotNull(handler.HandleFunc);

                Assert.Null(handler.AuthorizationInfo);

                Assert.NotNull(handler.FilteringInfo);
                Assert.False(handler.FilteringInfo!.IsEnabled);

                Assert.Null(handler.LimitingInfo);
                Assert.Null(handler.OpenApiInfo);
                Assert.Null(handler.RoutingInfo);

                var result = await handler.HandleAsync(new HandlerRequest
                {
                    HttpMethod = HandlerHttpMethod.Get
                });

                Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
                Assert.Equal("healthy", result.Data);
            }
            finally
            {
                descriptor.Release();
            }
        }

        [Fact]
        public void FromDescriptor_MapsAllOptionalHandlerMetadata()
        {
            var descriptor = CreateFullDescriptor();

            try
            {
                var handler = EndpointHandler.FromDescriptor(ref descriptor);

                Assert.Equal("/api/items/{id}", handler.Pattern);
                Assert.Equal(HandlerHttpMethod.Patch, handler.HttpMethod);

                Assert.NotNull(handler.AuthorizationInfo);
                Assert.Equal(
                    new[] { ClientRole.Administrator, ClientRole.User, ClientRole.Viewer },
                    handler.AuthorizationInfo!.RequiredRoles);

                Assert.NotNull(handler.FilteringInfo);
                Assert.True(handler.FilteringInfo!.IsEnabled);

                Assert.NotNull(handler.LimitingInfo);
                Assert.Equal(RateLimitingPolicy.Strict, handler.LimitingInfo!.RateLimitingPolicy);

                Assert.NotNull(handler.OpenApiInfo);
                Assert.Equal("GetItem", handler.OpenApiInfo!.Name);
                Assert.Equal("Get item", handler.OpenApiInfo.DisplayName);
                Assert.Equal(new[] { "items", "read" }, handler.OpenApiInfo.Tags);
                Assert.Equal("Returns an item", handler.OpenApiInfo.Summary);
                Assert.Equal("Items", handler.OpenApiInfo.GroupName);

                Assert.NotNull(handler.RoutingInfo);
                Assert.Equal(7, handler.RoutingInfo!.Priority);
                Assert.Equal(new[] { "api.example.org", "www.example.org" }, handler.RoutingInfo.Hosts);
            }
            finally
            {
                descriptor.Release();
            }
        }

        private static unsafe EndpointHandlerDescriptor CreateBaseDescriptor()
        {
            var pattern = AllocationUtilities.AllocString("/health", out var patternLength);

            var descriptor = new EndpointHandlerDescriptor
            {
                Pattern = pattern,
                PatternLength = patternLength,
                HttpMethod = (int)HandlerHttpMethod.Get
            };

            descriptor.SetHandler(_ => Task.FromResult(
                HandlerResult.Create(HttpStatusCode.OK).WithData("healthy")));

            return descriptor;
        }

        private static unsafe EndpointHandlerDescriptor CreateFullDescriptor()
        {
            var pattern = AllocationUtilities.AllocString("/api/items/{id}", out var patternLength);
            var name = AllocationUtilities.AllocString("GetItem", out var nameLength);
            var displayName = AllocationUtilities.AllocString("Get item", out var displayNameLength);
            var summary = AllocationUtilities.AllocString("Returns an item", out var summaryLength);
            var groupName = AllocationUtilities.AllocString("Items", out var groupNameLength);

            var tags = AllocationUtilities.AllocStringArray(["items", "read"], out var tagsCount);
            var hosts = AllocationUtilities.AllocStringArray(
                ["api.example.org", "www.example.org"],
                out var hostsCount);

            var descriptor = new EndpointHandlerDescriptor
            {
                Pattern = pattern,
                PatternLength = patternLength,
                HttpMethod = (int)HandlerHttpMethod.Patch,

                IsAuthorizedHandler = 1,
                RequiredRoles = (int)(ClientRole.Administrator | ClientRole.User | ClientRole.Viewer),

                IsFilteredHandler = 1,

                IsLimitedHandler = 1,
                RateLimitingPolicy = (int)RateLimitingPolicy.Strict,

                IsOpenApiHandler = 1,
                Name = name,
                NameLength = nameLength,
                DisplayName = displayName,
                DisplayNameLength = displayNameLength,
                Tags = tags,
                TagsCount = tagsCount,
                Summary = summary,
                SummaryLength = summaryLength,
                GroupName = groupName,
                GroupNameLength = groupNameLength,

                IsRoutedHandler = 1,
                Priority = 7,
                Hosts = hosts,
                HostsCount = hostsCount
            };

            descriptor.SetHandler(_ => Task.FromResult(
                HandlerResult.Create(HttpStatusCode.OK)));

            return descriptor;
        }
    }
}
