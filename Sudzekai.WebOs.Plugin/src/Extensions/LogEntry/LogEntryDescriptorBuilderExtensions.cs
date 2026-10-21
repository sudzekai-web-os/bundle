using Microsoft.Extensions.Logging.Abstractions;
using Sudzekai.WebOs.Core;

namespace Sudzekai.WebOs.Plugin
{
    internal static class LogEntryDescriptorBuilderExtensions
    {
        public static LogEntryDescriptor ToDescriptor<TState>(this LogEntry<TState> logEntry)
             => new()
             {
                 LogLevel = (int)logEntry.LogLevel,

                 Message = AllocationUtilities.AllocString(logEntry.Formatter(logEntry.State, logEntry.Exception), out var messageLength),
                 MessageLength = messageLength,

                 Category = AllocationUtilities.AllocString(logEntry.Category, out var categoryLength),
                 CategoryLength = categoryLength
             };
    }
}
