using Contracts.NotificationEvents;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using P7.NotificationService.Hubs;
using P7.NotificationService.Models;
using System.Text.Json;

namespace P7.NotificationService.Consumers
{
    public class SendNotificationBookingConsumer : IConsumer<SendNotification>
    {
        private ILogger<SendNotificationBookingConsumer> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;

        public SendNotificationBookingConsumer(ILogger<SendNotificationBookingConsumer> logger, IHubContext<NotificationHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<SendNotification> context)
        {
            var notification = context.Message;

            try
            {
                var notificationEntity = new Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = notification.UserId,
                    Type = notification.Type.ToString(),
                    Message = notification.Message,
                    Data = JsonSerializer.Serialize(notification.Data),
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                };

                // Send real time notification by using SignalR
                await _hubContext.Clients
                    .User(notification.UserId.ToString())
                    .SendAsync("ReceiveNotification", new
                    {
                        Id = notificationEntity.Id,
                        Type = notification.Type.ToString(),
                        Message = notification.Message,
                        Data = notification.Data,
                        CreatedAt = notificationEntity.CreatedAt
                    });
                _logger.LogInformation($"Notification sent to user {notification.UserId}: {notification.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send notification to user {notification.UserId}");
                throw;
            }
        }
    }
}
