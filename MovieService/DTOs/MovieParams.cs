using P1.MovieService.Models;

namespace P1.MovieService.DTOs
{
    public class MovieParams
    {
        public string SearchKey { get; set; }
        public int DurationMinutes { get; set; }
        public List<Genre> Genres { get; set; }
    }
}
