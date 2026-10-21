using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sudzekai.WebOs.Core;

namespace Sudzekai.WebOs.Plugin
{
    internal class DelegatedLogger(string categoryName, Action<LogEntryDescriptor> logAction) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
            => null;

        public bool IsEnabled(LogLevel logLevel)
            => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            LogEntry<TState> entry = new(logLevel, categoryName, eventId, state, exception, formatter);

            logAction.Invoke(entry.ToDescriptor());
        }
    }
}
