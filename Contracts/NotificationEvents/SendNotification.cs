namespace Contracts.NotificationEvents
{
    public record SendNotification(
        string UserId,
        NotificationType Type,
        string Message,
        object Data = null,
        DateTime? ScheduledTime = null
    );

    public enum NotificationType
    {
        BookingSuccess,
        BookingFailed
    }
}
