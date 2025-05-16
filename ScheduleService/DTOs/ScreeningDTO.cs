namespace ScheduleService.DTOs
{
    public class ScreeningDTO
    {
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }

        public Guid MovieId { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public int DurationMinutes { get; set; } = 0;
        public string PosterUrl { get; set; } = "";
        public string PublicId { get; set; } = "";
        public List<string> Genres { get; set; } = new();

        public Guid CinemaId { get; set; }
        public string CinemaName { get; set; } = "";
        public string CinemaAddress { get; set; } = "";
        public Guid RoomId { get; set; }
        public string RoomName { get; set; } = "";
        public int NumberOfSeats { get; set; } = 0;
    }
}
