namespace Sudzekai.LinCtrl.Hosting
{
    internal sealed record LoginToken(string AccessToken, string TokenType, int ExpiresIn);
}
