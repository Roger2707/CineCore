using CinemaService.Models;

namespace CinemaService.Repositories.IRepositories
{
    public interface ICinemaRepository
    {
        Task<List<Cinema>> GetAll();
        Task<Cinema> GetbyId(Guid id);
        Task Create(Cinema cinema);
        Task Delete(Guid id);
    }
}
