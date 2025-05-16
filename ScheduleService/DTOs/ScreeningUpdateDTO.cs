namespace ScheduleService.DTOs
{
    public class ScreeningUpdateDTO
    {
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public Guid MovieId { get; set; }
        public Guid CinemaId { get; set; }
        public Guid RoomId { get; set; }
    }
}
