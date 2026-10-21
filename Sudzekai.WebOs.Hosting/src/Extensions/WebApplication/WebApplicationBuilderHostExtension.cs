using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Sudzekai.WebOs.Hosting
{
    public static class WebApplicationBuilderHostExtension
    {
        extension (WebApplicationBuilder builder)
        {
            public void AddPluginInfrastructure()
            {
                builder.Services.AddSingleton<PluginsRegistry>();
                builder.Services.AddTransient<PluginLoader>();

                builder.Services.AddSingleton<PluginHost>();

                builder.Services.AddSingleton<PluginHandlersRegisterer>();
            }
        }
    }
}
