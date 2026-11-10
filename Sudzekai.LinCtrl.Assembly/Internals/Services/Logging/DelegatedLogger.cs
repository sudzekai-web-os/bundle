using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Abi.Managed.Types.Delegates;
using Microsoft.Extensions.Logging;
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Assembly.Internals.Services.Logging
{
    internal class DelegatedLogger(string category, LogDelegate del) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
            => null;

        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
            => true;

        public unsafe void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
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
