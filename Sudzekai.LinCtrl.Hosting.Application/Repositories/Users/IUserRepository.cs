using Sudzekai.LinCtrl.Hosting.Application.Objects.Models;

namespace Sudzekai.LinCtrl.Hosting.Application
{
    public interface IUserRepository
    {
        Task<User?> FindByUsernameAsync(string username, CancellationToken cancellationToken);
    }
}
