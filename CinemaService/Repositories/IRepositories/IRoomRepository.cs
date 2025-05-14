using CinemaService.DTOs;
using CinemaService.Models;

namespace CinemaService.Repositories.IRepositories
{
    public interface IRoomRepository
    {
        Task<List<RoomDTO>> GetAll(Guid cinemaId);
        Task<Room> GetbyId(Guid id);
        Task<RoomDTO> GetRoom(Guid id);
        Task Create(Room room);
        Task Delete(Guid id);
    }
}
