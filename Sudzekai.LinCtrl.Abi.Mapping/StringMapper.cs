using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Abi.Mapping
{
    public static class StringMapper
    {
        public static string ToManaged(StringViewer viewer)
            => viewer.Address == 0 ? string.Empty : Marshal.PtrToStringUTF8(viewer.Address, viewer.Length)!;
    }
}
