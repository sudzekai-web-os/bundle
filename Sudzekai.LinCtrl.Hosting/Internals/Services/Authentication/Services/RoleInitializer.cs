using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Sudzekai.LinCtrl.Core;
using Sudzekai.LinCtrl.Hosting.Application;
using Sudzekai.LinCtrl.Hosting.Application.Objects.Models;

namespace Sudzekai.LinCtrl.Hosting.Internals.Services.Authentication.Services
{
    internal sealed class RoleInitializer(IServiceScopeFactory scopes) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await using var scope = scopes.CreateAsyncScope();
            var roles = scope.ServiceProvider.GetRequiredService<IRoleRepository>();
            await roles.EnsureRolesAsync(
                [.. Enum.GetValues<ClientRole>().Select(role => new Role(role, role.ToString()))],
                cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
