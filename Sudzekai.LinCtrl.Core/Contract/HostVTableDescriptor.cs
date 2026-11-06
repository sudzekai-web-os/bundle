using System.Runtime.InteropServices;

namespace Sudzekai.LinCtrl.Core 
{
    [StructLayout(LayoutKind.Sequential)]
    public struct HostVTableDescriptor
    {
        public uint AbiVersion;
    }
}