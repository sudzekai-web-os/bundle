using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Core 
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct HttpRequestDescriptor : IMemFreeable
    {
        public int HttpMethod;

        public StringDescriptor* Host;

        public StringDescriptor* Scheme;

        public StringDescriptor* Path;

        public StringDescriptor* QueryString;

        public StringToStringArrayDictionaryDescriptor* Headers;

        public StringToStringArrayDictionaryDescriptor* Claims;

        public StringToStringDictionaryDescriptor* RouteValues;

        public StringToStringDictionaryDescriptor* Cookies;

        public int ContentType;

        public ByteArrayDescriptor* Body;

        public readonly bool IsAllocated =>
            Host != null ||
            Scheme != null ||
            Path != null ||
            QueryString != null ||
            Headers != null ||
            Claims != null ||
            RouteValues != null ||
            Cookies != null ||
            Body != null;

        public void Free()
        {
            AllocationUtilities.DeepFreeCoTaskMemIfNotNull(ref Host);
            AllocationUtilities.DeepFreeCoTaskMemIfNotNull(ref Scheme);
            AllocationUtilities.DeepFreeCoTaskMemIfNotNull(ref Path);
            AllocationUtilities.DeepFreeCoTaskMemIfNotNull(ref QueryString);

            AllocationUtilities.DeepFreeCoTaskMemIfNotNull(ref Headers);
            AllocationUtilities.DeepFreeCoTaskMemIfNotNull(ref Claims);

            AllocationUtilities.DeepFreeCoTaskMemIfNotNull(ref Cookies);
            AllocationUtilities.DeepFreeCoTaskMemIfNotNull(ref RouteValues);

            AllocationUtilities.DeepFreeCoTaskMemIfNotNull(ref Body);
        }
    }
}