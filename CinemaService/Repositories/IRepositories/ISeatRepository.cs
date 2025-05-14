using CinemaService.Models;

namespace CinemaService.Repositories.IRepositories
{
    public interface ISeatRepository
    {
        Task Create(List<Seat> seats);
    }
}
