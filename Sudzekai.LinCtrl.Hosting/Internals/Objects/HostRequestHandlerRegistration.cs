using Microsoft.AspNetCore.Builder;
using Sudzekai.LinCtrl.Abi.Managed;

namespace Sudzekai.LinCtrl.Hosting
{
    internal sealed record HostRequestHandlerRegistration(
        Func<IServiceProvider, IRequestHandler> Resolve,
        Action<EndpointBuilder> Configure,
        int MaxRequestBodyBytes);
}
