using MassTransit;
using PaymentService.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

#region Mass Transit

builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<PaymentRequestedConsumer>();
    
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint("payment-requested", e =>
        {
            e.ConfigureConsumer<PaymentRequestedConsumer>(context);
        });
        cfg.ConfigureEndpoints(context);
    });
});

#endregion

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
