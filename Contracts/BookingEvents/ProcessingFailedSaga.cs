namespace Contracts.BookingEvents
{
    public record ProcessingFailedSaga(Guid BookingId, List<Guid> Seats, Guid ScreeningId, string PaymentIntentId);
    public record FailedSagaEvent(Guid BookingId);
}
