using MassTransit;
using Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

#region MassTransit

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

#endregion

#region Authentication
builder.Services.AddSharedJwtAuthentication(builder.Configuration);
#endregion

var app = builder.Build();
app.MapGet("/", () => "Hello World!");
app.Run();
