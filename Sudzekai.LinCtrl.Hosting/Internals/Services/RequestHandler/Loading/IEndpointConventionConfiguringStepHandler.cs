using Microsoft.AspNetCore.Builder;

namespace Sudzekai.LinCtrl.Hosting 
{
    internal interface IEndpointConventionConfiguringStepHandler
    {
        void Handle(IEndpointConventionBuilder builder);
    }
}
