using System.Runtime.InteropServices;

namespace Sudzekai.WebOs.Core
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct EndpointHandlerDescriptor : IDisposable
    {
        // Base
        public nint Pattern; public int PatternLength;

        public int HttpMethod;

        public nint HandleFunc;


        // Authorized
        public int IsAuthorizedHandler;

        public int RequiredRoles;


        // Filtered
        public int IsFilteredHandler;


        // RateLimitng
        public int IsLimitedHandler;

        public int RateLimitingPolicy;


        // OpenApi
        public int IsOpenApiHandler;

        public nint Name; public int NameLength;

        public nint DisplayName; public int DisplayNameLength;

        public nint* Tags; public int TagsCount;

        public nint Summary; public int SummaryLength;

        public nint GroupName; public int GroupNameLength;


        // Routed
        public int IsRoutedHandler;

        public int Priority;

        public nint* Hosts; public int HostsCount;


        public void Dispose()
        {
            AllocationUtilities.FreeIfAllocated(ref Pattern);
            AllocationUtilities.FreeIfAllocated(ref Name);
            AllocationUtilities.FreeIfAllocated(ref DisplayName);
            AllocationUtilities.FreeIfAllocated(ref Summary);
            AllocationUtilities.FreeIfAllocated(ref GroupName);
            AllocationUtilities.FreeStringArray(Hosts, HostsCount);
            AllocationUtilities.FreeStringArray(Tags, TagsCount);
        }
    }
}
