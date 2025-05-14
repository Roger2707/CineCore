using CinemaService.DTOs;
using CinemaService.Models;

namespace CinemaService.Services.IServices
{
    public interface ICinemaService
    {
        Task<List<Cinema>> GetAll();
        Task<Cinema> GetbyId(Guid id);
        Task Create(CinemaCreateDTO cinemaCreateDTO);
        Task Update(Guid id, CinemaUpdateDTO cinemaUpdateDTO);
        Task Delete(Guid id);
    }
}
