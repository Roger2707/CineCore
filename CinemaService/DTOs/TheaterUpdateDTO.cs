using System.ComponentModel.DataAnnotations;

namespace P2.CinemaService.DTOs
{
    public class TheaterUpdateDTO
    {
        public string Name { get; set; }
        public Guid CinemaId { get; set; }
        [Range(1, 10)]
        public int TotalSeats { get; set; }
    }
}
