using CinemaService.DTOs;
using CinemaService.Models;

namespace CinemaService.Services.IServices
{
    public interface ITheaterService
    {
        Task<List<Theater>> GetAll(Guid cinemaId);
        Task<Theater> GetbyId(Guid id);

        Task Create(TheaterCreateDTO room);
        Task Update(Guid id, TheaterUpdateDTO room);
        Task Delete(Guid id);
    }
}
