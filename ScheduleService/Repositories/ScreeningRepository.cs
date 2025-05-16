using Microsoft.EntityFrameworkCore;
using ScheduleService.Data;
using ScheduleService.DTOs;
using ScheduleService.Models;
using ScheduleService.Repositories.IRepositories;

namespace ScheduleService.Repositories
{
    public class ScreeningRepository : IScreeningRepository
    {
        private readonly ScheduleDBContext _db;
        public ScreeningRepository(ScheduleDBContext db)
        {
            _db = db;
        }

        #region CRUD

        public async Task Create(Screening screening)
        {
            await _db.Screenings.AddAsync(screening);
        }

        public async Task Delete(Guid id)
        {
           var screening = await _db.Screenings.FindAsync(id);
           if (screening == null) throw new Exception("Screening not found");
           _db.Screenings.Remove(screening);
        }

        #endregion

        #region Retrieve
        public Task<List<ScreeningDTO>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<ScreeningDTO> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<Screening> GetScreeing(Guid id)
        {
            return await _db.Screenings.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task SaveChangeAsync()
        {
            await _db.SaveChangesAsync();
        }

        #endregion
    }
}
