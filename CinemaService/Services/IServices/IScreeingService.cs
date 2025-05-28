using P2.CinemaService.DTOs;
using P2.CinemaService.Models;
using Contracts.BookingEvents;

namespace P2.CinemaService.Services.IServices
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
