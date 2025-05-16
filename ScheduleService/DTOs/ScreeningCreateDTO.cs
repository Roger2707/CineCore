using System.ComponentModel.DataAnnotations;

namespace ScheduleService.DTOs
{
    public class ScreeningCreateDTO
    {
        [Required]
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        [Required]
        public Guid MovieId { get; set; }
        [Required]
        public Guid CinemaId { get; set; }
        [Required]
        public Guid RoomId { get; set; }
    }
}
