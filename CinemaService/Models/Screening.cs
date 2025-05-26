using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaService.Models
{
    public class Screening
    {
        public Guid Id { get; set; }
        public Guid MovieId { get; set; }

        public Guid CinemaId { get; set; }
        [ForeignKey("CinemaId")]
        public Cinema Cinema { get; set; }
        public Guid TheaterId { get; set; }
        [ForeignKey("TheaterId")]
        public Theater Theater { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Duration { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
