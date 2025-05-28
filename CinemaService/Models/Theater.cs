using System.ComponentModel.DataAnnotations.Schema;

namespace P2.CinemaService.Models
{
    public class Theater
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int TotalSeats { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public Guid CinemaId { get; set; }
        [ForeignKey("CinemaId")]
        public Cinema Cinema { get; set; }
    }
}
