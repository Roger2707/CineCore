namespace BookingService.DTOs
{
    public class BookingCreateRequestDTO
    {
        public Guid UserId { get; set; }
        public Guid ScreeningId { get; set; }
        public List<Guid> Seats { get; set; }
        public string PaymentIntentId { get; set; }
    }
}
