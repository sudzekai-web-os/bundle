
namespace Sudzekai.LinCtrl.Core 
{
    public readonly unsafe ref struct MemScope<TDescripor> : IDisposable
        where TDescripor : unmanaged, IMemFreeable
    {
        private readonly ref TDescripor* _source;

        public MemScope(ref TDescripor* source)
            => _source = ref source;

        public readonly void Dispose()
            => AllocationUtilities.DeepFreeCoTaskMemIfNotNull(ref _source);
    }
}
