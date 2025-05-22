using MassTransit;

namespace Contracts.BookingEvents
{
    public class BookingCreated : CorrelatedBy<Guid>
    {
        public Guid BookingId { get; set; }
        public List<Guid> SeatIds { get; set; }
        public Guid ScreeningId { get; set; }
        public Guid CorrelationId => BookingId;
    }
    public record BookingFailed(Guid BookingId, List<Guid> SeatIds, Guid ScreeningId);
    public record BookingFinished(Guid BookingId, BookingStatus BookingStatus);
    public enum BookingStatus
    {
        PENDING, SUCCESSED, CANCELLED, EXPIRED
    }
}
