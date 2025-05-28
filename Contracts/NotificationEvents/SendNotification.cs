namespace Contracts.NotificationEvents
{
    public record SendNotification(
        Guid UserId,
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
