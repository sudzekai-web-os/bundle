using Sudzekai.LinCtrl.Abi;
using Sudzekai.LinCtrl.Abi.Allocation;
using Sudzekai.LinCtrl.Abi.Managed;

namespace Sudzekai.LinCtrl.Hosting.Internals.Extensions.Abi
{
    internal static class HostVTableAllocatorExtensions
    {
        extension (HostVTableAllocator)
        {
            public static HostVTableDescriptor Alloc(HostVTable vTable)
            {
                return HostVTableAllocator.Alloc(vTable.AbiVersion);
            }
        }
    }
}
