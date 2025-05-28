using P1.MovieService.Models;
using System.ComponentModel.DataAnnotations;

namespace P1.MovieService.DTOs
{
    public class MovieCreateDTO
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int DurationMinutes { get; set; } = 100;
        [Required]
        public IFormFile PosterFile { get; set; }
        [Required]
        public List<Genre> Genres { get; set; } = new List<Genre>();
    }
}
