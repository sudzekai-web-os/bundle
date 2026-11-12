namespace Sudzekai.LinCtrl.Hosting.Application
{
    public interface IHashService
    {
        string Hash(string password);
        bool Verify(string password, string? passwordHash);
    }
}