using Sudzekai.WebOs.Core;

namespace Sudzekai.WebOs.Plugin
{
    internal static class EndpointHandlerDescriptorDisposeExtensions
    {
        extension(EndpointHandlerDescriptor descriptor)
        {
            public void Release()
                => EndpointHandlerCallbacksContainer.Free(descriptor.HandleFunc);
        }
    }
}
