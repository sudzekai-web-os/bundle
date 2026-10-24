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

                Assert.False(handler.IsAuthorizedHandler);
                Assert.False(handler.IsFilteredHandler);
                Assert.False(handler.IsLimitedHandler);
                Assert.False(handler.IsOpenApiHandler);
                Assert.False(handler.IsRoutedHandler);

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

                Assert.True(handler.IsAuthorizedHandler);
                Assert.Equal(
                    new[] { ClientRole.Administrator, ClientRole.User, ClientRole.Viewer },
                    handler.RequiredRoles);

                Assert.True(handler.IsFilteredHandler);

                Assert.True(handler.IsLimitedHandler);
                Assert.Equal(RateLimitingPolicy.Strict, handler.RateLimitingPolicy);

                Assert.True(handler.IsOpenApiHandler);
                Assert.Equal("GetItem", handler.Name);
                Assert.Equal("Get item", handler.DisplayName);
                Assert.Equal(new[] { "items", "read" }, handler.Tags);
                Assert.Equal("Returns an item", handler.Summary);
                Assert.Equal("Items", handler.GroupName);

                Assert.True(handler.IsRoutedHandler);
                Assert.Equal(7, handler.Priority);
                Assert.Equal(new[] { "api.example.org", "www.example.org" }, handler.Hosts);
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
