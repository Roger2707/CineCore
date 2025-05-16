using ScheduleService.DTOs;

namespace ScheduleService.Services.IServices
{
    public interface IScreeningService
    {
        Task<List<ScreeningDTO>> GetAll();
        Task<ScreeningDTO> GetById(Guid id);
        Task Create(ScreeningCreateDTO screening);
        Task Update(Guid id, ScreeningUpdateDTO screening);
        Task Delete(Guid id);
    }
}
