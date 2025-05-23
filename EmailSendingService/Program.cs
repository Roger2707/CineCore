using EmailSendingService.Consumers;
using EmailSendingService.Services;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

#region Mass Transit Configuration

builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<EmailTicketCreatedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint("email-ticket-created", e =>
        {
            e.ConfigureConsumer<EmailTicketCreatedConsumer>(context);
        });
        cfg.ConfigureEndpoints(context);
    });
});

#endregion


builder.Services.AddScoped<EmailService>();

app.MapGet("/", () => "Hello World!");

app.Run();
