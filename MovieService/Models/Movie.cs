namespace P1.MovieService.Models
{
    public class Movie
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DurationMinutes { get; set; }
        public string PosterUrl { get; set; }
        public string PublicId { get; set; }
        public List<Genre> Genres { get; set; }
    }
}