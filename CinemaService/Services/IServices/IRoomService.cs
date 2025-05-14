using CinemaService.DTOs;
using CinemaService.Models;

namespace CinemaService.Services.IServices
{
    public interface IRoomService
    {
        Task<List<RoomDTO>> GetAll(Guid cinemaId);
        Task<Room> GetbyId(Guid id);
        Task<RoomDTO> GetRoom(Guid id);

        Task Create(RoomCreateDTO room);
        Task UpdateRoom(Guid id, RoomUpdateDTO room);
        Task DeleteRoom(Guid id);
    }
}
