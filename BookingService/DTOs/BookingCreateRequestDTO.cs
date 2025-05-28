using System.ComponentModel.DataAnnotations;

namespace P4.BookingService.DTOs
{
    public class BookingCreateRequestDTO
    {
        public Guid BookingId { get; set; }
        public Guid UserId { get; set; }
        public Guid ScreeningId { get; set; }
        [Range(1, 3)]
        public List<Guid> Seats { get; set; }
        public string PaymentIntentId { get; set; }
    }
}
