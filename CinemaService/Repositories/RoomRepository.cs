using CinemaService.Data;
using CinemaService.DTOs;
using CinemaService.Models;
using CinemaService.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace CinemaService.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly CinemaDBContext _db;
        public RoomRepository(CinemaDBContext db)
        {
            _db = db;
        }

        public async Task Create(Room room)
        {
           await _db.Rooms.AddAsync(room);
        }

        public async Task Delete(Guid id)
        {
            var room = await _db.Rooms.FirstOrDefaultAsync(x => x.Id == id);
            _db.Rooms.Remove(room);
        }

        public async Task<List<RoomDTO>> GetAll(Guid cinemaId)
        {
            string query = @"
                        SELECT 
                            r.Id, r.Name, c.Name as CinemaName, SELECT COUNT(1) FROM Seats WHERE RoomId = r.Id as TotalSeats
                        FROM Rooms r 
                        INNER JOIN Cinemas c ON r.CinemaId = c.Id
                        WHERE c.Id = @CinemaId
                        ";
            var parameters = new { CinemaId = cinemaId }; 
            var rooms = await _db.Database.SqlQueryRaw<RoomDTO>(query, parameters).ToListAsync();
            return rooms;
        }

        public async Task<Room> GetbyId(Guid id)
        {
            var room = await _db.Rooms.Include(x => x.Cinema).Include(x => x.Seats).FirstOrDefaultAsync(x => x.Id == id);
            return room;
        }

        public async Task<RoomDTO> GetRoom(Guid id)
        {
            string query = @"
                        SELECT 
                            r.Id, r.Name, c.Name as CinemaName, SELECT COUNT(1) FROM Seats WHERE RoomId = r.Id as TotalSeats
                        FROM Rooms r 
                        INNER JOIN Cinemas c ON r.CinemaId = c.Id
                        WHERE r.Id = @RoomId
                        ";
            var parameters = new { RoomId = id };
            var room = await _db.Database.SqlQueryRaw<RoomDTO>(query, parameters).FirstOrDefaultAsync();
            return room;
        }
    }
}
