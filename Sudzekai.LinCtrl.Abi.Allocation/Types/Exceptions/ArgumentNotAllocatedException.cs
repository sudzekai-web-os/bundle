using Sudzekai.LinCtrl.Abi.Abstractions;

namespace Sudzekai.LinCtrl.Abi.Allocation.Types.Exceptions
{
    public class ArgumentNotAllocatedException(string message) : Exception(message)
    {
        public static void ThrowIfNotAllocated(IAllocationState value, string parameterName)
        {
            if (!value.IsAllocated)
                throw new ArgumentNotAllocatedException($"Value must be allocated. (Parameter: '{parameterName}')");
        }
    }
}
