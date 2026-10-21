namespace Sudzekai.WebOs.Core
{
    public interface IOpenApiEndpointHandler : IEndpointHandler
    {
        string Name { get; }

        string DisplayName { get; }
        
        string[] Tags { get; }
        
        string Summary { get; }
        
        string GroupName { get; }
    }
}
