namespace Contracts.BookingEvents
{
    public record ProcessingFailedSaga(Guid BookingId, List<Guid> Seats, Guid ScreeningId, string PaymentIntentId, Guid UserId);
    public record FailedSagaEvent(Guid BookingId, Guid UserId);
}
