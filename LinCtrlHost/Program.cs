using Serilog;
using Sudzekai.LinCtrl.Hosting;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Configuration.AddJsonFile("local/appsettings.json", optional: true);
builder.Configuration.AddEnvironmentVariables();
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

