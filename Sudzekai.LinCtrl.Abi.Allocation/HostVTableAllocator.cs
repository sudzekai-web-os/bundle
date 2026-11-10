namespace Sudzekai.LinCtrl.Abi.Allocation
{
    public static class HostVTableAllocator
    {
        public static HostVTableDescriptor Alloc(uint abiVersion)
            => new(abiVersion);

        public static void Free(ref HostVTableDescriptor descriptor)
        {
            if (!descriptor.IsAllocated)
                return;
        }
    }
}
