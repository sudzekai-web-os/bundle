using Sudzekai.WebOs.Core.src.Endpoint.Request.Descriptors;
using System.Runtime.InteropServices;

namespace Sudzekai.WebOs.Core
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct HandlerRequestDescriptor : IDisposable
    {
        public int HttpMethod;

        public nint Host; public int HostLength;

        public nint Scheme; public int SchemeLength;

        public nint Path; public int PathLength;

        public nint QueryString; public int QueryStringLength;

        public StringArrayEntryDescriptor* Headers; public int HeadersCount;

        public StringEntryDescriptor* RouteValues; public int RouteValuesCount;

        public StringEntryDescriptor* Cookies; public int CookiesCount;
        
        public StringArrayEntryDescriptor* Claims; public int ClaimsCount; 

        public nint ContentType; public int ContentTypeLength;

        public nint Body; public int BodyLength;

        public void Dispose()
        {
            AllocationUtilities.FreeIfAllocated(ref Host);
            AllocationUtilities.FreeIfAllocated(ref Scheme);
            AllocationUtilities.FreeIfAllocated(ref Path);
            AllocationUtilities.FreeIfAllocated(ref QueryString);
            AllocationUtilities.FreeIfAllocated(ref ContentType);
            AllocationUtilities.FreeIfAllocated(ref Body);

            FreeStringArrayEntries(Headers, HeadersCount);
            FreeStringArrayEntries(Claims, ClaimsCount);

            FreeStringEntries(RouteValues, RouteValuesCount);
            FreeStringEntries(Cookies, CookiesCount);
        }

        private static void FreeStringArrayEntries(StringArrayEntryDescriptor* entries, int count)
        {
            if (entries == null)
                return;

            for (int i = 0; i < count; i++)
                (&entries[i])->Dispose(); // обнуление полей реального элемента

            Marshal.FreeCoTaskMem((nint)entries);
        }

        private static void FreeStringEntries(StringEntryDescriptor* entries, int count)
        {
            if (entries == null)
                return;

            for (int i = 0; i < count; i++)
                (&entries[i])->Dispose(); // обнуление полей реального элемента

            Marshal.FreeCoTaskMem((nint)entries);
        }
    }
}
