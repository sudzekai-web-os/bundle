namespace Sudzekai.WebOs.Core
{
    public class HandlerAuthorizationInfo
    {
        public HandlerAuthorizationInfo(ClientRole[] requiredRoles)
        {
            RequiredRoles = requiredRoles;
        }

        public ClientRole[] RequiredRoles { get; } = [];
    }
}
