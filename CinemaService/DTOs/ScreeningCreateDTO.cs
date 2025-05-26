using System.ComponentModel.DataAnnotations;

namespace CinemaService.DTOs
{
    public class ScreeningCreateDTO
    {
        [Required]
        public Guid MovieId { get; set; }
        [Required]
        public Guid CinemaId { get; set; }
        [Required]
        public Guid TheaterId { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
    }
}
