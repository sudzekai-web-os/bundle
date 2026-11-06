using LinCtrlAssembly.Context;
using LinCtrlAssembly.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Sudzekai.LinCtrl.Assembly;
using Sudzekai.LinCtrl.Core;

namespace LinCtrlAssembly
{
    [PluginAssembly]
    public partial class LinCtrlTestPlugin : IPluginAssembly
    {
        public void Configure(AssemblyHostBuilder builder)
        {
            LinCtrlContext.AddTypeInfoResolver(AssemblyJsonContext.Default);

            builder.Services.AddScoped<IHttpRequestHandler, ResponseObjectRequestHandler>();
            builder.Services.AddScoped<IHttpRequestHandler, UnregisteredResponseObjectHandler>();
        }
    }
}
