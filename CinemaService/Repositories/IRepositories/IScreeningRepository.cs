using CinemaService.Models;

namespace CinemaService.Repositories.IRepositories
{
    public interface IScreeningRepository
    {
        Task<List<Screening>> GetAll(Guid cinemaId);
        Task<Screening> GetById(Guid id);
        Task Create(Screening screening);
        Task Delete(Guid id);
    }
}
