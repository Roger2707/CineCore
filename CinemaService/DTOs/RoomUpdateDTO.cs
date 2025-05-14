using System.ComponentModel.DataAnnotations;

namespace CinemaService.DTOs
{
    public class RoomUpdateDTO
    {
        public string Name { get; set; }
        public Guid CinemaId { get; set; }
        [Range(1, 10)]
        public int NumberOfRow { get; set; }
    }
}
