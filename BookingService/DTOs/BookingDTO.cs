namespace BookingService.DTOs
{
    public class BookingDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ScreeningId { get; set; }
        public string BookingStatus { get; set; }
        public List<BookingSeatDTO> Items { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
