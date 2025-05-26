using Contracts.BookingEvents;

namespace BookingService.Models
{
    public class Booking
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ScreeningId { get; set; }
        public BookingStatus BookingStatus { get; set; } = BookingStatus.PENDING;
        public decimal TotalPrice { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public string PaymentIntentId { get; set; }
    }
}
