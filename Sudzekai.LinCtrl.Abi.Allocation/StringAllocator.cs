using System.Runtime.InteropServices;
using System.Text;

namespace Sudzekai.LinCtrl.Abi.Allocation
{
    public static class StringAllocator
    {
        public static StringViewer Alloc(string source)
        {
            var bytes = Encoding.UTF8.GetBytes(source);

            var stringAddress = Marshal.AllocCoTaskMem(bytes.Length + 1);

            try
            {
                Marshal.Copy(bytes, 0, stringAddress, bytes.Length);
                Marshal.WriteByte(stringAddress, bytes.Length, 0);
            }
            catch
            {
                Marshal.FreeCoTaskMem(stringAddress);
                throw;
            }

            return new(stringAddress, bytes.Length);
        } 

        public static void Free(ref StringViewer viewer)
        {
            if (!viewer.IsAllocated)
                return;

            Marshal.FreeCoTaskMem(viewer.Address);
            viewer = default;
        }
    }
}
