using CinemaService.DTOs;
using CinemaService.Models;
using Contracts.BookingEvents;

namespace CinemaService.Services.IServices
{
    public interface IScreeingService
    {
        Task<List<Screening>> GetAll(Guid theaterId);
        Task<Screening> GetbyId(Guid id);
        Task Create(ScreeningCreateDTO screeningCreateDTO);
        Task Update(Guid id, ScreeningUpdateDTO screeningUpdateDTO);
        Task Delete(Guid id);
        Task UpdateScreeningSeatStatus(UpdateSeatStatus updateSeatStatus);
    }
}
