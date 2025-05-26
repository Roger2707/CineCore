using CinemaService.Data;
using CinemaService.Models;
using CinemaService.Repositories.IRepositories;
using Contracts.BookingEvents;
using Microsoft.EntityFrameworkCore;

namespace CinemaService.Repositories
{
    public class ScreeningRepository : IScreeningRepository
    {
        private readonly CinemaDBContext _db;

        public ScreeningRepository(CinemaDBContext db)
        {
            _db = db;
        }

        public async Task Create(Screening screening)
        {
            await _db.Screenings.AddAsync(screening);
        }

        public async Task Delete(Guid id)
        {
            var screening = await _db.Screenings.FindAsync(id);
            _db.Screenings.Remove(screening);
        }

        public async Task<List<Screening>> GetAll(Guid cinemaId)
        {
            return await _db.Screenings
                .Where(s => s.CinemaId == cinemaId)
                .Include(s => s.Cinema)
                .Include(s => s.Theater)
                .ToListAsync();
        }

        public Task<Screening> GetById(Guid id)
        {
            return _db.Screenings
                .Include(s => s.Cinema)
                .Include(s => s.Theater)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task UpdateScreeningSeatStatus(UpdateSeatStatus updateSeatStatus)
        {
            foreach(var seat in updateSeatStatus.Seats)
            {
                await _db.ScreeningSeats.AddAsync(new ScreeningSeat
                {
                    SeatId = seat,
                    ScreeningId = updateSeatStatus.ScreeningId,
                    SeatStatus = updateSeatStatus.SeatStatus,
                    ReservedAt = DateTime.UtcNow,
                    ReservedBy = updateSeatStatus.UserId
                });
            }
        }
    }
}
