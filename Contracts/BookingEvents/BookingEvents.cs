﻿using MassTransit;

namespace Contracts.BookingEvents
{
    #region Booking events
    public class BookingCreated : CorrelatedBy<Guid>
    {
        public Guid BookingId { get; set; }
        public List<Guid> SeatIds { get; set; }
        public Guid ScreeningId { get; set; }
        public Guid UserId { get; set; }
        public string PaymentIntentId { get; set; }
        public Guid CorrelationId => BookingId;
    }
    public record BookingFinished(Guid BookingId, BookingStatus BookingStatus);
    public enum BookingStatus{ PENDING, CONFIRMED, CANCELLED, REFUNDED }
    public record BookingCreateCommand(Guid BookingId, Guid UserId, Guid ScreeningId, List<Guid> SeatIds, string PaymentIntentId);
    #endregion

    #region Others support events
    public record TicketDelivered(Guid BookingId, Guid UserId);
    public record EmailTicketCreated(Guid BookingId, string UserEmail, Guid UserId);
    public record PaymentRefund(Guid BookingId, Guid ScreeningId, List<Guid> Seats, string PaymentIntentId);
    #endregion
}
