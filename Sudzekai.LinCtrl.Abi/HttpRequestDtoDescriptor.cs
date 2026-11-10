using Sudzekai.LinCtrl.Abi.Abstractions;
using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Abi
{
    [StructLayout(LayoutKind.Sequential)]
    public struct HttpRequestDtoDescriptor(
        int httpMethod,
        StringViewer host,
        StringViewer scheme,
        StringViewer path,
        StringViewer queryString,
        StringToStringArrayDictionaryDescriptor headers,
        StringToStringArrayDictionaryDescriptor claims,
        StringToStringDictionaryDescriptor routeValues,
        StringToStringDictionaryDescriptor cookies,
        int contentType,
        ByteArrayViewer body) : IAllocationState
    {
        public int HttpMethod = httpMethod;

        public StringViewer Host = host;

        public StringViewer Scheme = scheme;

        public StringViewer Path = path;

        public StringViewer QueryString = queryString;

        public StringToStringArrayDictionaryDescriptor Headers = headers;

        public StringToStringArrayDictionaryDescriptor Claims = claims;

        public StringToStringDictionaryDescriptor RouteValues = routeValues;

        public StringToStringDictionaryDescriptor Cookies = cookies;

        public int ContentType = contentType;

        public ByteArrayViewer Body = body;

        public readonly bool IsAllocated => Host.IsAllocated 
            || Scheme.IsAllocated 
            || Path.IsAllocated 
            || QueryString.IsAllocated 
            || Headers.IsAllocated 
            || Claims.IsAllocated 
            || RouteValues.IsAllocated 
            || Cookies.IsAllocated 
            || Body.IsAllocated;
    }
}
