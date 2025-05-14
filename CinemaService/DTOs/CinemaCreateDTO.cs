using System.ComponentModel.DataAnnotations;

namespace CinemaService.DTOs
{
    public class CinemaCreateDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
    }
}
