namespace Contracts.BookingEvents
{
    public record SeatHoldRequested(Guid BookingId);
    public record SeatHoldCompleted(Guid BookingId);
    public record SeatHoldFailed(Guid BookingId);
}
