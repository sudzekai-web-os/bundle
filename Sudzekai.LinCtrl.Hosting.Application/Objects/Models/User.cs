using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Hosting.Application.Objects.Models
{
    public sealed record User(
        long Id,
        string Username,
        string PasswordHash,
        ClientRole Role,
        bool IsBlocked);
}
