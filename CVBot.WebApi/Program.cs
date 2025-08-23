using CVBot.WebApi;

var builder = WebApplication.CreateBuilder(args);

var startup = new StartupHelper();
startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app, app.Environment);

app.Run();