namespace Sudzekai.LinCtrl.Hosting.Domain.Authentication
{
    public interface IUserRepository
    {
        Task<User?> FindByUsernameAsync(string username, CancellationToken cancellationToken);
    }
}
