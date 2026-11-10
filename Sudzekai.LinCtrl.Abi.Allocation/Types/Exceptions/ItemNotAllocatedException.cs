using Sudzekai.LinCtrl.Abi.Abstractions;

namespace Sudzekai.LinCtrl.Abi.Allocation.Types.Exceptions
{
    public class ItemNotAllocatedException(string message) : Exception(message)
    {
        public static void ThrowIfNotAllocated<T>(ReadOnlySpan<T> source)
            where T : IAllocationState
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (!source[i].IsAllocated)
                    throw new ItemNotAllocatedException($"All items of an array must be allocated. (Issued index: {i})");
            }
        }
    }
}
