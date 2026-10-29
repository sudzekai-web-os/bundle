using Microsoft.AspNetCore.Builder;
using Sudzekai.WebOs.Core;

namespace Sudzekai.WebOs.Hosting
{
    internal interface IHandlerConfigurationStepHandler
    {
        string Handle(IEndpointConventionBuilder builder, IEndpointHandler handler);
    }
}
