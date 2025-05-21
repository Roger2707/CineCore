namespace Contracts.BookingEvents
{
    public record BookingCreated(Guid BookingId, List<Guid> SeatIds, Guid ScreeningId);
    public record BookingFailed(Guid BookingId, List<Guid> SeatIds, Guid ScreeningId);
}
