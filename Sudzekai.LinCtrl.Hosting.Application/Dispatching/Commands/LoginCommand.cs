namespace Sudzekai.LinCtrl.Hosting.Application
{
    public sealed record LoginCommand(string Username, string Password) : ICommand<LoginResult>;
}
