using Sudzekai.LinCtrl.Hosting.Application.Objects.Models;

namespace Sudzekai.LinCtrl.Hosting.Application
{
    public sealed record LoginResult(User? User, bool IsBlocked = false);
}
