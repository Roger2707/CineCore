using EmailSendingService.Consumers;
using EmailSendingService.Services;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<EmailService>();

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

var app = builder.Build();
app.MapGet("/", () => "Hello World!");

app.Run();
