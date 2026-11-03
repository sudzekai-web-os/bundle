
namespace Sudzekai.LinCtrl.Core 
{
    public static unsafe class LogEntryDescriptorExtensions
    {
        extension(LogEntryDescriptor descriptor)
        {
            public static LogEntryDescriptor Alloc(
                StringDescriptor category,
                StringDescriptor message,
                int logLevel)
            {
                StringDescriptor* categoryPointer = null;
                StringDescriptor* messagePointer = null;

                try
                {
                    categoryPointer = AllocationUtilities.AllocCoTaskMem(category);
                    messagePointer = AllocationUtilities.AllocCoTaskMem(message);
                }
                catch
                {
                    AllocationUtilities.FreeCoTaskMemIfNotNull(ref categoryPointer);
                    AllocationUtilities.FreeCoTaskMemIfNotNull(ref messagePointer);

                    throw;
                }

                return new()
                {
                    Category = categoryPointer,
                    Message = messagePointer,
                    LogLevel = logLevel
                };
            }

            public static LogEntryDescriptor Alloc(
                string category,
                string message,
                LogLevel logLevel)
            {
                StringDescriptor categoryDescriptor = default;
                StringDescriptor messageDescriptor = default;

                try
                {
                    categoryDescriptor = StringDescriptor.Alloc(category);
                    messageDescriptor = StringDescriptor.Alloc(message);

                    return Alloc(categoryDescriptor, messageDescriptor, (int)logLevel);
                }
                catch
                {
                    categoryDescriptor.Free();
                    messageDescriptor.Free();

                    throw;
                }
            }

            public static LogEntryDescriptor Alloc(LogEntry entry)
            {
                ArgumentNullException.ThrowIfNull(entry);

                return Alloc(entry.Category, entry.Message, entry.LogLevel);
            }

            public LogEntry ToManaged()
            {
                var category = descriptor.Category is null
                    ? string.Empty : descriptor.Category->ToManaged();

                var message = descriptor.Message is null
                    ? string.Empty : descriptor.Message->ToManaged();

                return new()
                {
                    Category = category,
                    Message = message,
                    LogLevel = (LogLevel)descriptor.LogLevel
                };
            }
        }
    }
}