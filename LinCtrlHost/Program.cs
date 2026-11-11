using Serilog;
using Sudzekai.LinCtrl.Hosting;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Configuration.AddJsonFile("local/appsettings.json", optional: true);
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddCommandLine(args);

if (!int.TryParse(builder.Configuration["LinCtrl:Port"], out var port) || port is < 1 or > 65535)
    throw new InvalidOperationException("LinCtrl:Port must be an integer between 1 and 65535");

builder.WebHost.ConfigureKestrel(options => options.ListenLocalhost(port));
builder.AddLinCtrlInfrastructure();

if (builder.Environment.IsDevelopment())
    builder.Services.AddOpenApi();

builder.Logging.ClearProviders();

builder.Services.AddSerilog(cfg => cfg
    .MinimumLevel.Information()
    .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss.fff} {Level:u3} {Message:lj}{NewLine}{Exception}"));

var app = builder.Build();

app.MapLinCtrlInfrastructure();

app.LoadPlugins();
app.InitializePlugins();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.Run();

