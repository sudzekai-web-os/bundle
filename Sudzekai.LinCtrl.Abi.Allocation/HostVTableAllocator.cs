namespace Sudzekai.LinCtrl.Abi.Allocation
{
    public static class HostVTableAllocator
    {
        public static HostVTableDescriptor Alloc(uint abiVersion)
            => new(abiVersion);

        public static HostVTableDescriptor Alloc(uint abiVersion, IEnumerable<KeyValuePair<string, string?>> configuration)
            => new(abiVersion, StringToStringDictionaryAllocator.AllocNullableValues(configuration));

        public static void Free(ref HostVTableDescriptor descriptor)
        {
            StringToStringDictionaryAllocator.Free(ref descriptor.Configuration);
            descriptor = default;
        }
    }
}
