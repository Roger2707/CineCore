using CinemaService.Models;
using Contracts.BookingEvents;

namespace CinemaService.Repositories.IRepositories
{
    public interface IScreeningRepository
    {
        Task<List<Screening>> GetAll(Guid cinemaId);
        Task<Screening> GetById(Guid id);
        Task Create(Screening screening);
        Task Delete(Guid id);
        Task UpdateScreeningSeatStatus(UpdateSeatStatus updateSeatStatus);
    }
}
