using Sudzekai.LinCtrl.Hosting;

var builder = WebApplication.CreateSlimBuilder(args);

builder.AddLinCtrlInfrastructure();

var app = builder.Build();

app.MapLinCtrlInfrastructure();

app.LoadPlugins();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.Run();

