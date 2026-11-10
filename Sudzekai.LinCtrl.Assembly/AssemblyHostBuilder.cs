using Sudzekai.LinCtrl.Abi.Managed.Types.Delegates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sudzekai.LinCtrl.Assembly.Context;
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Assembly
{
    public class AssemblyHostBuilder
    {
        private readonly ServiceCollection _services = new();
        private bool _built;

        internal AssemblyHostBuilder(AssemblyContext ctx)
        {
        }

        public IServiceCollection Services { get => _services; }

        public void DelegateLogging(LogDelegate del)
        {
            ArgumentNullException.ThrowIfNull(del);

            Services.AddLogging(b => b.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace));
            Services.AddSingleton<ILoggerProvider>(new DelegatedLoggerProvider(del));
        }

        public AssemblyHost Build()
        {
            if (_built)
                throw new InvalidOperationException("Assembly host already built");

            _built = true;
            _services.MakeReadOnly();

            return new(Services.BuildServiceProvider());
        }
    }
}
