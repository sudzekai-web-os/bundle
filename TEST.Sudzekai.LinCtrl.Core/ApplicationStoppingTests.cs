using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Sudzekai.LinCtrl.Abi.Managed;
using Sudzekai.LinCtrl.Core;
using Sudzekai.LinCtrl.Hosting;

namespace TEST.Sudzekai.LinCtrl.Core
{
    public class ApplicationStoppingTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task StoppingDisposesEveryPluginBeforeServiceProviderDisposal(bool fail)
        {
            LinCtrlContext.AddTypeInfoResolver(HostingContext.Default);
            using var lifetime = new TestApplicationLifetime();
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddSingleton<IHostApplicationLifetime>(lifetime);
            services.RegisterRequestHandlerServices();
            services.AddAssemblyServices();
            services.AddPluginServices();
            services.RemoveAll<IAssemblyLoader>();
            var loader = new TestAssemblyLoader(fail);
            services.AddSingleton<IAssemblyLoader>(loader);
            var provider = services.BuildServiceProvider();
            var directory = Directory.CreateTempSubdirectory("LinCtrlStoppingTests-");
            try
            {
                var service = Assert.Single(provider.GetServices<IHostedService>());
                await service.StartAsync(TestContext.Current.CancellationToken);
                var host = provider.GetRequiredService<PluginLifetimeHost>();
                List<PluginInfo> plugins = [];
                for (int i = 0; i < 2; i++)
                {
                    var path = Directory.CreateDirectory(Path.Combine(directory.FullName, i.ToString()));
                    File.WriteAllText(Path.Combine(path.FullName, "manifest.json"),
                        "{ \"name\": \"test\", \"assemblies\": [\"module.dll\"] }");
                    File.WriteAllBytes(Path.Combine(path.FullName, "module.dll"), []);
                    plugins.Add(host.Load(path.FullName));
                }

                Assert.Equal(0, loader.DisposalCount);
                lifetime.StopApplication();

                Assert.Equal(2, loader.DisposalCount);
                Assert.All(plugins, plugin => Assert.Equal(PluginLifetime.Disposed, plugin.Lifetime));
                Assert.Empty(host.GetPlugins());
                lifetime.StopApplication();
                await service.StopAsync(TestContext.Current.CancellationToken);
                provider.Dispose();
                Assert.Equal(2, loader.DisposalCount);
            }
            finally
            {
                try
                {
                    provider.Dispose();
                }
                finally
                {
                    directory.Delete(true);
                }
            }
        }

        private sealed class TestAssemblyLoader(bool fail) : IAssemblyLoader
        {
            public int DisposalCount { get; private set; }

            public void Load(AssemblyInfo info)
            {
                info.VTable = new AssemblyVTable
                {
                    AssemblyDisposeDelegate = () =>
                    {
                        DisposalCount++;
                        if (fail && DisposalCount == 1)
                            throw new InvalidOperationException("Disposal failed");
                    }
                };
            }
        }

        private sealed class TestApplicationLifetime : IHostApplicationLifetime, IDisposable
        {
            private readonly CancellationTokenSource _stopping = new();

            public CancellationToken ApplicationStarted => CancellationToken.None;

            public CancellationToken ApplicationStopping => _stopping.Token;

            public CancellationToken ApplicationStopped => CancellationToken.None;

            public void StopApplication()
                => _stopping.Cancel();

            public void Dispose()
                => _stopping.Dispose();
        }
    }
}
