using Microsoft.Extensions.Logging;
using Sudzekai.LinCtrl.Assembly.Internals.Services.Logging;
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Assembly
{
    internal class DelegatedLoggerProvider(LogDelegate del) : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
            => new DelegatedLogger(categoryName, del);

        public void Dispose() { }
    }
}
