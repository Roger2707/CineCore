namespace P2.CinemaService.DTOs
{
    public class MovieDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int DurationMinutes { get; set; }
        public string PosterUrl { get; set; } = null!;
        public string PublicId { get; set; } = null!;
        public List<string> Genres { get; set; } = new List<string>();
    }
}
