using P2.CinemaService.Models;

namespace P2.CinemaService.Repositories.IRepositories
{
    public interface ITheaterRepository
    {
        Task<List<Theater>> GetAll(Guid cinemaId);
        Task<Theater> GetbyId(Guid id);
        Task Create(Theater room);
        Task Delete(Guid id);
    }
}
