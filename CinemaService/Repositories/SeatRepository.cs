using CinemaService.Data;
using CinemaService.Models;
using CinemaService.Repositories.IRepositories;

namespace CinemaService.Repositories
{
    public class SeatRepository : ISeatRepository
    {
        private readonly CinemaDBContext _db;
        public SeatRepository(CinemaDBContext db)
        {
            _db = db;
        }
        public async Task Create(List<Seat> seats)
        {
            await _db.Seats.AddRangeAsync(seats);
        }
    }
}
