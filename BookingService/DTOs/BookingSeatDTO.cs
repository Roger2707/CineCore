namespace P4.BookingService.DTOs
{
    public class BookingSeatDTO
    {
        public Guid Id { get; set; }
        public Guid SeatId { get; set; }
        public string SeatInfo { get; set; }
        public decimal Price { get; set; }
    }
}
