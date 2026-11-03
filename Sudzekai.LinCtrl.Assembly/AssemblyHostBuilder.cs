using Microsoft.Extensions.DependencyInjection;

namespace Sudzekai.LinCtrl.Assembly
{
    public class AssemblyHostBuilder
    {
        private readonly ServiceCollection _services = new();

        public IServiceCollection Services { get => _services; }

        internal AssemblyHostBuilder() { }

        public AssemblyHost Build()
        {
            _services.MakeReadOnly();

            return new(Services.BuildServiceProvider());
        }
    }
}
