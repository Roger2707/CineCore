using MassTransit;
using PaymentService.Consumers;
using PaymentService.Services.IServices;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

#region Mass Transit

builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<PaymentRefundConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint("refund-payment", e =>
        {
            e.ConfigureConsumer<PaymentRefundConsumer>(context);
        });
        cfg.ConfigureEndpoints(context);
    });
});

#endregion

#region Redis

var redisConnectionString = builder.Configuration.GetValue<string>("Redis:ConnectionString");
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));

#endregion

#region Services

builder.Services.AddScoped<IPaymentService, PaymentService.Services.PaymentService>();

#endregion

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
