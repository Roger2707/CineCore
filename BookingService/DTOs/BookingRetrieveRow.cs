namespace P4.BookingService.DTOs
{
    public class BookingRetrieveRow
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ScreeningId { get; set; }
        public string BookingStatus { get; set; }
        public Guid BookingSeatId { get; set; }
        public Guid SeatId { get; set; }
        public string SeatInfo { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
