using P2.CinemaService.DTOs;
using P2.CinemaService.Models;

namespace P2.CinemaService.Services.IServices
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
