using SSMTP.Server;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHostedService<Server>();

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();
