using Contracts.BookingEvents;

namespace CinemaService.Models
{
    public class ScreeningSeat
    {
        public Guid ScreeningId { get; set; }
        public Guid SeatId { get; set; }
        public SeatStatus SeatStatus { get; set; } = SeatStatus.AVAILABLE;
        public Guid? ReservedBy { get; set; } // UserId
        public DateTime? ReservedAt { get; set; }
        public DateTime? BookedAt { get; set; }
    }
}
