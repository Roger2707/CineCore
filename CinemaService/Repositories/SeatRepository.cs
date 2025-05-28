using P2.CinemaService.Data;
using P2.CinemaService.Models;
using P2.CinemaService.Repositories.IRepositories;

namespace P2.CinemaService.Repositories
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
