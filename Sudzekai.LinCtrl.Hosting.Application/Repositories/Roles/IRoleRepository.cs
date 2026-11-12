using Sudzekai.LinCtrl.Hosting.Application.Objects.Models;

namespace Sudzekai.LinCtrl.Hosting.Application
{
    public interface IRoleRepository
    {
        Task EnsureRolesAsync(IReadOnlyList<Role> roles, CancellationToken cancellationToken);
    }
}
