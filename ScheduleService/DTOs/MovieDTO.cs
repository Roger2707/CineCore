namespace ScheduleService.DTOs
{
    public class MovieDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DurationMinutes { get; set; }
        public string PosterUrl { get; set; }
        public string PublicId { get; set; }
        public List<string> Genres { get; set; }
    }
}
