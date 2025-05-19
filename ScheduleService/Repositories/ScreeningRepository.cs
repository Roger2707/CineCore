using Microsoft.Data.SqlClient;
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
        public async Task<List<ScreeningDTO>> GetAll()
        {
            string query = @"
                            SELECT Id, StartTime, MovieId, CinemaId, RoomId 
                            FROM Screenings
                            ";
            var screenings = await _db.Database.SqlQueryRaw<Screening>(query).ToListAsync();
            return screenings.Select(s => new ScreeningDTO
            {
                Id = s.Id,
                StartTime = s.StartTime,
                MovieId = s.MovieId,
                CinemaId = s.CinemaId,
                RoomId = s.RoomId
            }).ToList();
        }

        public async Task<ScreeningDTO> GetById(Guid id)
        {
            string query = @"
                            SELECT Id, StartTime, MovieId, CinemaId, RoomId 
                            FROM Screenings
                            WHERE Id = @ScreeningId
                            ";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@ScreeningId", id)
            };
            var screening = await _db.Database.SqlQueryRaw<Screening>(query, parameters).FirstOrDefaultAsync();
            return new ScreeningDTO
            {
                Id = screening.Id,
                StartTime = screening.StartTime,
                MovieId = screening.MovieId,
                CinemaId = screening.CinemaId,
                RoomId = screening.RoomId
            };
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
