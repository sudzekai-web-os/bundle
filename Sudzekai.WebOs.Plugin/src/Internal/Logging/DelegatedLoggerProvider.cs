using Microsoft.Extensions.Logging;
using Sudzekai.WebOs.Core;

namespace Sudzekai.WebOs.Plugin
{
    internal class DelegatedLoggerProvider(Action<LogEntryDescriptor> logAction) : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
            => new DelegatedLogger(categoryName, logAction);

        public void Dispose() { }
    }
}
