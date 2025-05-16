namespace ScheduleService.Models
{
    public class Screening
    {
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }

        // Other Services Key
        public Guid MovieId { get; set; }
        public Guid CinemaId { get; set; }
        public Guid RoomId { get; set; }
    }
}
