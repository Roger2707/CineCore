namespace P2.CinemaService.DTOs
{
    public class CinemaBaseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class CinemaExtraDTO : CinemaBaseDTO
    {
        public DateTime Created { get; set; }
    }
}
