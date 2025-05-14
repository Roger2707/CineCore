using CinemaService.DTOs;
using CinemaService.Models;
using CinemaService.Repositories.IRepositories;
using CinemaService.Services.IServices;

namespace CinemaService.Services
{
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoomService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Retrieve
        public async Task<List<RoomDTO>> GetAll(Guid cinemaId)
        {
            return await _unitOfWork.Room.GetAll(cinemaId);
        }

        public async Task<Room> GetbyId(Guid id)
        {
            return await _unitOfWork.Room.GetbyId(id);
        }

        public async Task<RoomDTO> GetRoom(Guid id)
        {
            return await _unitOfWork.Room.GetRoom(id);
        }

        #endregion

        #region CRUD 

        public async Task Create(RoomCreateDTO roomCreateDTO)
        {
            var cinemaExisted = await _unitOfWork.Cinema.GetbyId(roomCreateDTO.CinemaId) != null;
            if (!cinemaExisted) throw new Exception("Cinema not found");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var room = new Room
                {
                    Name = roomCreateDTO.Name,
                    CinemaId = roomCreateDTO.CinemaId
                };

                await _unitOfWork.SaveChangesAsync();

                for (int i = 0; i < roomCreateDTO.NumberOfRow; i++)
                {
                    char rowLetter = (char)('A' + i);

                    for (int col = 1; col <= 10; col++)
                    {
                        room.Seats.Add(new Seat
                        {
                            Row = rowLetter.ToString(),
                            Number = col,
                            RoomId = room.Id,
                        });
                    }
                }

                await _unitOfWork.Room.Create(room);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch(Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }          
        }

        public Task UpdateRoom(Guid id, RoomUpdateDTO room)
        {
            throw new NotImplementedException();
        }

        public Task DeleteRoom(Guid id)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
