using Microsoft.Extensions.Logging;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Abi.Managed.Types.Delegates;

namespace Sudzekai.LinCtrl.Assembly.Internals.Services.Logging
{
    internal class DelegatedLogger(string category, LogDelegate del) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
            => null;

        public bool IsEnabled(LogLevel logLevel)
            => true;

        public unsafe void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var entry = new LogEntryDto()
            {
                LogLevel = logLevel,
                Category = category,
                Message = formatter(state, exception)
            };

            del(entry);
        }
    }
}
