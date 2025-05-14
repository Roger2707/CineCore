using System.ComponentModel.DataAnnotations;

namespace CinemaService.DTOs
{
    public class RoomCreateDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public Guid CinemaId { get; set; }
        [Range(1, 10)]
        public int NumberOfRow { get; set; }
    }
}
