using Sudzekai.WebOs.Core;

namespace Sudzekai.WebOs.Plugin
{
    internal interface IOpenApiMetadataWriter
    {
        string Write(IReadOnlyList<IEndpointHandler> handlers);
    }
}