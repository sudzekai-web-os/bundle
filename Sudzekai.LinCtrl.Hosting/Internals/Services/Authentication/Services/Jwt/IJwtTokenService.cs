using Sudzekai.LinCtrl.Hosting.Application.Objects.Models;

namespace Sudzekai.LinCtrl.Hosting
{
    internal interface IJwtTokenService
    {
        LoginResponse Issue(User user);
    }
}