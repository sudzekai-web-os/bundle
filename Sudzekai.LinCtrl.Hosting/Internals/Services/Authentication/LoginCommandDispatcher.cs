using Sudzekai.LinCtrl.Hosting.Domain.Authentication;

namespace Sudzekai.LinCtrl.Hosting
{
    internal sealed class LoginCommandDispatcher(
        IUserRepository users,
        PasswordHashService passwords,
        JwtTokenService tokens)
    {
        public async Task<LoginToken?> DispatchAsync(LoginCommand command, CancellationToken cancellationToken)
        {
            var user = await users.FindByUsernameAsync(command.Username, cancellationToken);
            var valid = passwords.Verify(command.Password, user?.PasswordHash);
            if (!valid || user is null || user.IsBlocked || !Enum.IsDefined(user.Role))
                return null;

            return tokens.Issue(user);
        }
    }
}
