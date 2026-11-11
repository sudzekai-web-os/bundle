namespace Sudzekai.LinCtrl.Hosting.Domain.Authentication
{
    public interface IRoleRepository
    {
        Task EnsureRolesAsync(IReadOnlyList<Role> roles, CancellationToken cancellationToken);
    }
}
