namespace Sudzekai.LinCtrl.Hosting.Application
{
    internal sealed class LoginCommandHandler(IUserRepository users, IHashService passwords)
        : ICommandHandler<LoginCommand, LoginResult>
    {
        public async Task<LoginResult> HandleAsync(LoginCommand command, CancellationToken cancellationToken)
        {
            var user = await users.FindByUsernameAsync(command.Username, cancellationToken);
            var valid = passwords.Verify(command.Password, user?.PasswordHash);
            if (!valid || user is null || !Enum.IsDefined(user.Role))
                return new(null);

            return user.IsBlocked ? new(null, IsBlocked: true) : new(user);
        }
    }
}
