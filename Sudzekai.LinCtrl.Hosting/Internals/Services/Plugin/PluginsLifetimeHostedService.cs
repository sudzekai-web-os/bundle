using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Sudzekai.LinCtrl.Hosting
{
    internal sealed class PluginsLifetimeHostedService(
        PluginsLifetimeHost host,
        IHostApplicationLifetime lifetime,
        ILogger<PluginsLifetimeHostedService> logger) : IHostedService, IDisposable
    {
        private CancellationTokenRegistration _stoppingRegistration;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _stoppingRegistration = lifetime.ApplicationStopping.Register(DisposePlugins);
            return Task.CompletedTask;
        }

        private void DisposePlugins()
        {
            try
            {
                host.Dispose(host.GetPlugins());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception while disposing plugins on application stopping");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;

        public void Dispose()
            => _stoppingRegistration.Dispose();
    }
}
