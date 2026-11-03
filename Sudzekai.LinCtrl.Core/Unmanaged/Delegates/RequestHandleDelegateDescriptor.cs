using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Core 
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct RequestHandleDelegateDescriptor
        : IMemFreeable
    {
        public delegate* unmanaged[Cdecl]<HttpRequestDescriptor*, HttpResponseDescriptor*> Delegate;

        public delegate* unmanaged[Cdecl]<void> FreeAction;

        public readonly bool IsAllocated => Delegate != null && FreeAction != null;

        public void Free()
        {
            if (FreeAction == null)
                return;

            FreeAction();
            FreeAction = null;
            Delegate = null;
        }
    }
}