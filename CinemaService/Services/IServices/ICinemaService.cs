using P2.CinemaService.DTOs;
using P2.CinemaService.Models;

namespace P2.CinemaService.Services.IServices
{
    public interface ICinemaService
    {
        Task<List<Cinema>> GetAll();
        Task<CinemaBaseDTO> GetbyId(Guid id);
        Task Create(CinemaCreateDTO cinemaCreateDTO);
        Task Update(Guid id, CinemaUpdateDTO cinemaUpdateDTO);
        Task Delete(Guid id);
    }
}
