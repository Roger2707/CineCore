using ScheduleService.DTOs;
using ScheduleService.Models;

namespace ScheduleService.Repositories.IRepositories
{
    public interface IScreeningRepository
    {
        Task<List<ScreeningDTO>> GetAll();
        Task<ScreeningDTO> GetById(Guid id);
        Task<Screening> GetScreeing(Guid id);
        Task Create(Screening screening);
        Task Delete(Guid id);
        Task SaveChangeAsync();
    }
}
