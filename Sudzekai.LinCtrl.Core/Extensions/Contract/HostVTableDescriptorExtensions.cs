namespace Sudzekai.LinCtrl.Core 
{
    public static class HostVTableDescriptorExtensions
    {
        extension(HostVTableDescriptor descriptor)
        {
            public static HostVTableDescriptor Alloc(HostVTable source)
                => new() { AbiVersion = source.AbiVersion };
        }
    }
}