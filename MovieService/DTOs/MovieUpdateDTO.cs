using P1.MovieService.Models;

namespace P1.MovieService.DTOs
{
    public class MovieUpdateDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int DurationMinutes { get; set; } = 100;
        public IFormFile? PosterFile { get; set; }
        public List<Genre> Genres { get; set; } = new List<Genre>();
    }
}
