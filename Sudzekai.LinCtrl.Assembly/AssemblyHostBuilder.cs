using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sudzekai.LinCtrl.Assembly.Context;
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Assembly
{
    public class AssemblyHostBuilder
    {
        private readonly AssemblyContext _context;
        private readonly ServiceCollection _services = new();

        internal AssemblyHostBuilder(AssemblyContext ctx)
        {
            _context = ctx;


        }

        public IServiceCollection Services { get => _services; }

        public int DelegateLogging(LogDelegate del)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(del);

                Services.AddLogging(b => b.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace));

                Services.AddSingleton<ILoggerProvider>(new DelegatedLoggerProvider(del));

                return 0;
            }
            catch
            {
                return 1;
            }
        }

        public AssemblyHost Build()
        {
            _services.MakeReadOnly();

            return new(Services.BuildServiceProvider());
        }
    }
}
