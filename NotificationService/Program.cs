using MassTransit;
using P7.NotificationService.Consumers;
using P7.NotificationService.Hubs;
using Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

#region MassTransit

builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<SendNotificationBookingConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint("notification-booking-service", e =>
        {
            e.ConfigureConsumer<SendNotificationBookingConsumer>(context);
        });

        cfg.ConfigureEndpoints(context);
    });
});

#endregion

builder.Services.AddSignalR();

#region Authentication
builder.Services.AddSharedJwtAuthentication(builder.Configuration);
#endregion

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

#region SignalR
app.MapHub<NotificationHub>("/notificationHub");
#endregion

app.Run();
