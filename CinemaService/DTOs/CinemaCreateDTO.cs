using System.ComponentModel.DataAnnotations;

namespace P2.CinemaService.DTOs
{
    public class CinemaCreateDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
    }
}
