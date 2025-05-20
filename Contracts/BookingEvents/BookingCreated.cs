namespace Contracts.BookingEvents
{
    public record BookingCreated(Guid BookingId, Guid UserId, decimal TotalPrice);
}
