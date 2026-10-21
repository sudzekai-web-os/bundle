namespace Sudzekai.WebOs.Core
{
    public interface IAuthorizedEndpointHandler : IEndpointHandler
    {
        ClientRole[] RequiredRoles { get; }
    }
}
