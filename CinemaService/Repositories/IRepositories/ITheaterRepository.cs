using CinemaService.DTOs;
using CinemaService.Models;

namespace CinemaService.Repositories.IRepositories
{
    public interface ITheaterRepository
    {
        Task<List<Theater>> GetAll(Guid cinemaId);
        Task<Theater> GetbyId(Guid id);
        Task Create(Theater room);
        Task Delete(Guid id);
    }
}
