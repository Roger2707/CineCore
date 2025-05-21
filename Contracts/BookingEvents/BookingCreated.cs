using MassTransit;

namespace Contracts.BookingEvents
{
    public record BookingCreated(Guid BookingId, List<Guid> SeatIds, Guid ScreeningId) : CorrelatedBy<Guid>
    {
        public Guid CorrelationId => BookingId;
    }

    public record BookingFailed(Guid BookingId, List<Guid> SeatIds, Guid ScreeningId);
}
