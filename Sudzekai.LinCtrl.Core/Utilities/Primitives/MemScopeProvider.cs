
namespace Sudzekai.LinCtrl.Core 
{
    public unsafe class MemScopeProvider
    {
        public static MemScope<TDescriptor> CreateScope<TDescriptor>(ref TDescriptor* descriptor)
            where TDescriptor : unmanaged, IMemFreeable
            => new(ref descriptor);
    }
}
