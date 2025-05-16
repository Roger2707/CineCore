namespace ScheduleService.DTOs
{
    public class RoomDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Cinema { get; set; }
        public int TotalSeats { get; set; }
    }
}
