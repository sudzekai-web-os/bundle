using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Hosting
{
    internal static class HostingContext
    {
        public const uint AbiVersion = 1;

        private static HostVTable _vTable = new(AbiVersion);

        public unsafe static HostVTableDescriptor* VTableDescriptorPointer = AllocationUtilities.AllocCoTaskMem(HostVTableDescriptor.Alloc(_vTable));
    }
}