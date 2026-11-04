using Microsoft.AspNetCore.Builder;
using Sudzekai.LinCtrl.Core;

namespace Sudzekai.LinCtrl.Hosting
{
    public static class WebApplicationBuilderExtensions
    {
        extension(WebApplicationBuilder builder)
        {
            public void AddLinCtrlInfrastructure()
            {
                CoreContext.JsonSerializerOptions.TypeInfoResolverChain.Add(HostingJsonContext.Default);
                builder.Services.AddAssemblyServices();
                builder.Services.RegisterRequestHandlerServices();
                builder.Services.AddPluginServices();
            }
        }
    }
}
