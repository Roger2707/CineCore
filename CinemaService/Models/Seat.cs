namespace CinemaService.Models
{
    public class Seat
    {
        public Guid Id { get; set; }
        public string Row { get; set; }
        public int Number { get; set; }
        public Guid RoomId { get; set; }
        public Room Room { get; set; }
        public SeatStatus SeatStatus { get; set; } = SeatStatus.Empty;
    }
}
