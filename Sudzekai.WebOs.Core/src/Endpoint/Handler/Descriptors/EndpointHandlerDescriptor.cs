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


        // Routed
        public int IsRoutedHandler;

        public int Priority;

        public nint* Hosts; public int HostsCount;


        public void Dispose()
        {
            AllocationUtilities.FreeIfAllocated(ref Pattern);
            AllocationUtilities.FreeStringArray(Hosts, HostsCount);
        }
    }
}
