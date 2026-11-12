namespace Sudzekai.LinCtrl.Hosting
{
    public record LoginResponse(
        string Token,
        string Username,
        long Id,
        int RoleId,
        int ExpiresIn,
        string TokenType = "Bearer");
}
