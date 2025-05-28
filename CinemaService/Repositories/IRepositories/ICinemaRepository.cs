using P2.CinemaService.Models;

namespace P2.CinemaService.Repositories.IRepositories
{
    public interface ICinemaRepository
    {
        Task<List<Cinema>> GetAll();
        Task<Cinema> GetbyId(Guid id);
        Task Create(Cinema cinema);
        Task Delete(Guid id);
    }
}
