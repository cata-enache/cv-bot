using CVBot.WebApi;

var builder = WebApplication.CreateBuilder(args);

var startup = new StartupHelper(builder.Configuration);
#pragma warning disable SKEXP0010
startup.ConfigureServices(builder.Services);
#pragma warning restore SKEXP0010

var app = builder.Build();

startup.Configure(app, app.Environment);

app.Run();