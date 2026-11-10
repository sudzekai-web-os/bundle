using Sudzekai.LinCtrl.Abi.Allocation.Types.Exceptions;

namespace Sudzekai.LinCtrl.Abi.Allocation
{
    public static class LogEntryDtoAllocator
    {
        public static LogEntryDtoDescriptor Alloc(
            StringViewer category,
            StringViewer message,
            int logLevel)
        {
            ArgumentNotAllocatedException.ThrowIfNotAllocated(category, nameof(category));
            ArgumentNotAllocatedException.ThrowIfNotAllocated(message, nameof(message));

            return new(category, message, logLevel);
        }

        public static LogEntryDtoDescriptor Alloc(
            string category,
            string message,
            int logLevel)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(category, nameof(category));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(message, nameof(message));

            StringViewer categoryViewer = default;
            StringViewer messageViewer = default;

            try
            {
                categoryViewer = StringAllocator.Alloc(category);
                messageViewer = StringAllocator.Alloc(message);

                return Alloc(categoryViewer, messageViewer, logLevel);
            }
            catch
            {
                StringAllocator.Free(ref categoryViewer);
                StringAllocator.Free(ref messageViewer);
                
                throw;
            }
        }
    }
}
