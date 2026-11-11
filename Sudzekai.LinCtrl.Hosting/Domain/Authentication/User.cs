using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Hosting.Domain.Authentication
{
    public sealed record User(long Id, string Username, string PasswordHash, ClientRole Role, bool IsBlocked);
}
