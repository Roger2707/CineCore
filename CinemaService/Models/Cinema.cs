namespace CinemaService.Models
{
    public class Cinema
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public List<Room> Rooms { get; set; }
    }
}
