using Sudzekai.WebOs.Core;
using Sudzekai.WebOs.Hosting;
using Sudzekai.WebOs.Hosting.src.Extensions.ServiceCollection;
using System.Diagnostics.CodeAnalysis;
using TestPluginHost.ConsoleFormatting;
using TestPluginHost.Handlers;

namespace TestPluginHost
{
    public class Program
    {
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ColorfulConsoleFormatter))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ColorfulConsoleFormatterOptions))]
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.ConfigureRateLimiting();

            builder.Services.AddAuthentication();
            builder.Services.AddAuthorization();
            
            builder.AddPluginInfrastructure();

            builder.Logging.AddConsoleFormatter<ColorfulConsoleFormatter, ColorfulConsoleFormatterOptions>();

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole(c =>
            {
                c.FormatterName = "Toolkit";
            });

            builder.Services.AddScoped<IEndpointHandler, InternalHandler>();

            var app = builder.Build();

            app.UseAuthentication();
            app.UseAuthorization();

            await app.LoadPluginsAsync();

            app.AddHandlers();

            var logger = app.Services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("Загруженнные сборки:\n\t{plugins}", 
                string.Join("\n\t", app.GetPluginsList().Select((p, i) => 
                $"   {i+1}: {Path.GetFileName(Path.GetDirectoryName(p.AssemblyPath))}/{Path.GetFileName(p.AssemblyPath)} " +
                $"| {p.Info.Name} " +
                $"| ver. {p.Info.Version} " +
                $"| обработчиков: {p.Handlers.Length} " +
                $"| {p.Info.Summary}")));

            await app.StartAsync();

            await app.WaitForShutdownAsync();
        }
    }
}
