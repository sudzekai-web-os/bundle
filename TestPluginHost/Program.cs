using Scalar.AspNetCore;
using Sudzekai.WebOs.Core;
using Sudzekai.WebOs.Hosting;
using System.Diagnostics;
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

            builder.Services.AddOpenApi();

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

            app.MapScalarApiReference();

            app.UseAuthentication();
            app.UseAuthorization();

            await app.LoadPluginsAsync();

            app.MapHandlers();
            app.MapOpenApi();

            var logger = app.Services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("Загруженнные сборки:\n\t{plugins}", 
                string.Join("\n\t", app.GetPluginsList().Select((p, i) => 
                $"   {i+1}: {Path.GetFileName(Path.GetDirectoryName(p.AssemblyPath))}/{Path.GetFileName(p.AssemblyPath)} " +
                $"| {p.Info.Name} " +
                $"| ver. {p.Info.Version} " +
                $"| обработчиков: {p.Handlers.Count} " +
                $"| {p.Info.Summary}")));

            await app.StartAsync();

            OpenSwaggerUi(app);

            await app.WaitForShutdownAsync();
        }

        private static void OpenSwaggerUi(WebApplication app)
        {
            var address = app.Urls.FirstOrDefault(u => u.StartsWith("http://"))
                          ?? app.Urls.FirstOrDefault()
                          ?? "http://localhost:5000";

            var url = address.TrimEnd('/') + "/scalar/v1";

            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception)
            {
            }
        }
    }
}
