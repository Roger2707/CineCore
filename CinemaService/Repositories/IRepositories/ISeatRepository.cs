using P2.CinemaService.Models;

namespace P2.CinemaService.Repositories.IRepositories
{
    public interface ISeatRepository
    {
        Task Create(List<Seat> seats);
    }
}
