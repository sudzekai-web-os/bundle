
namespace Sudzekai.LinCtrl.Core 
{
    public unsafe struct FreeAssemblyVTableDelegateDescriptor
        : IMemFreeable
    {
        public delegate* unmanaged[Cdecl]<AssemblyVTableDescriptor*, int> Delegate;

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