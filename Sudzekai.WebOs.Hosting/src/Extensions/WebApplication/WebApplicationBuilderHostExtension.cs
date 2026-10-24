using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Sudzekai.WebOs.Hosting.src.Objects;

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
            }
        }
    }
}
