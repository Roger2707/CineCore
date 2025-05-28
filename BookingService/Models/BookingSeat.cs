using System.ComponentModel.DataAnnotations.Schema;

namespace P4.BookingService.Models
{
    public class BookingSeat
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        [ForeignKey("BookingId")]
        public Booking Booking { get; set; }
        public Guid SeatId { get; set; }
        public decimal Price { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
