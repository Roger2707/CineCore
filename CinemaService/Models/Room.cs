namespace CinemaService.Models
{
    public class Room
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CinemaId { get; set; }
        public Cinema Cinema { get; set; }
        public List<Seat> Seats { get; set; } = new List<Seat>();
    }
}
