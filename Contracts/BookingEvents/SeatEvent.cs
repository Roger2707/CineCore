namespace Contracts.BookingEvents
{
    public enum SeatStatus { AVAILABLE, RESERVED, BOOKED }
    public record UpdateSeatStatus(Guid BookingId, Guid ScreeningId, List<Guid> Seats, SeatStatus SeatStatus, Guid UserId, string PaymentIntentId);
    public record SeatUpdateCompleted(Guid BookingId, Guid ScreeningId, List<Guid> Seats, string PaymentIntentId);
    public record SeatUpdateFailed(Guid BookingId, Guid ScreeningId, List<Guid> Seats, string PaymentIntentId);
    public record ReleaseSeatHold(Guid ScreeningId, List<Guid> Seats);
}
