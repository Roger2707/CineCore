namespace Contracts.BookingEvents
{
    public record PaymentRequested(Guid BookingId);
    public record PaymentCompleted(Guid BookingId);
    public record PaymentFailed(Guid BookingId);
}
