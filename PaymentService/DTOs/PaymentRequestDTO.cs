namespace P5.PaymentService.DTOs
{
    public class PaymentRequestDTO
    {
        public Guid UserId { get; set; }
        public Guid ScreeningId { get; set; }
        public List<Guid> Seats { get; set; }
    }
}
