using System.Runtime.InteropServices;
using System.Text;

namespace Sudzekai.LinCtrl.Core 
{
    public static class StringDescriptorExtensions
    {
        extension(StringDescriptor descriptor)
        {
            public static StringDescriptor Alloc(string str)
            {
                var bytes = Encoding.UTF8.GetBytes(str);

                var stringPointer = Marshal.AllocCoTaskMem(bytes.Length + 1);

                try
                {
                    Marshal.Copy(bytes, 0, stringPointer, bytes.Length);
                    Marshal.WriteByte(stringPointer, bytes.Length, 0);
                }
                catch
                {
                    Marshal.FreeCoTaskMem(stringPointer);
                    throw;
                }

                return new()
                {
                    Pointer = stringPointer,
                    Length = bytes.Length
                };
            }

            public string ToManaged()
            {
                if (descriptor.Pointer == 0 || descriptor.Length <= 0)
                    return string.Empty;

                var bytes = new byte[descriptor.Length];

                Marshal.Copy(descriptor.Pointer, bytes, 0, descriptor.Length);

                return Encoding.UTF8.GetString(bytes);
            }
        }
    }
}