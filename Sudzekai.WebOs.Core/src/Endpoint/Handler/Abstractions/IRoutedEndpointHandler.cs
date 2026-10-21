namespace Sudzekai.WebOs.Core
{
    public interface IRoutedEndpointHandler : IEndpointHandler
    {
        int Priority { get; }

        string[] Hosts { get; }
    }
}
