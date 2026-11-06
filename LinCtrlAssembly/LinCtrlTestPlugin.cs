using LinCtrlAssembly.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Sudzekai.LinCtrl.Assembly;
using Sudzekai.LinCtrl.Core;

namespace LinCtrlAssembly
{
    [PluginAssembly(typeof(LinCtrlTestPlugin))]
    public partial class LinCtrlTestPlugin : IPluginAssembly
    {
        public void ConfigureBuilder(AssemblyHostBuilder builder)
        {
            builder.Services.AddScoped<IHttpRequestHandler, ResponseObjectRequestHandler>();
            builder.Services.AddScoped<IHttpRequestHandler, UnregisteredResponseObjectHandler>();
        }
    }
}
