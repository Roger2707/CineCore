using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaService.Models
{
    public class Seat
    {
        public Guid Id { get; set; }
        public string RowName { get; set; } // A, B, C, etc.
        public int SeatNumber { get; set; } // 1, 2, 3, etc.
        public bool IsActive { get; set; } = true;
        public Guid TheaterId { get; set; }
        [ForeignKey("TheaterId")]
        public Theater Theater { get; set; }
    }
}
