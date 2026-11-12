using Microsoft.Extensions.Logging;
using Sudzekai.LinCtrl.Abi.Managed.Types.Delegates;
using Sudzekai.LinCtrl.Assembly.Internals.Services.Logging;

namespace Sudzekai.LinCtrl.Assembly
{
    internal class DelegatedLoggerProvider(LogDelegate del) : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
            => new DelegatedLogger(categoryName, del);

        public void Dispose() { }
    }
}
