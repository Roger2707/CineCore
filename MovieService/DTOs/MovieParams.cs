using MovieService.Models;

namespace MovieService.DTOs
{
    public class MovieParams
    {
        public string SearchKey { get; set; }
        public int DurationMinutes { get; set; }
        public List<Genre> Genres { get; set; }
    }
}
