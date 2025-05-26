namespace CinemaService.DTOs
{
    public class ScreeningUpdateDTO
    {
        public Guid? MovieId { get; set; }
        public Guid? CinemaId { get; set; }
        public Guid? TheaterId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
