using ScheduleService.DTOs;
using ScheduleService.Models;
using ScheduleService.Repositories.IRepositories;
using ScheduleService.Services.IServices;

namespace ScheduleService.Services
{
    public class ScreeningService : IScreeningService
    {
        private readonly IScreeningRepository _screeningRepository;

        public ScreeningService(IScreeningRepository screeningRepository)
        {
            _screeningRepository = screeningRepository;
        }

        #region Validations

        private bool IsMovieExisted(Guid movieId)
        {

            return true;
        }

        private bool IsCinemaExisted(Guid cinemaId)
        {
            return true;
        }

        private bool IsRoomExisted(Guid roomId)
        {
            return true;
        }

        #endregion

        #region CRUD

        public async Task Create(ScreeningCreateDTO screeningCreate)
        {
            if(IsMovieExisted(screeningCreate.MovieId) == false) throw new Exception("Movie not found");
            if(IsCinemaExisted(screeningCreate.CinemaId) == false) throw new Exception("Cinema not found");
            if(IsRoomExisted(screeningCreate.RoomId) == false) throw new Exception("Room not found");

            var screening = new Screening
            {
                StartTime = screeningCreate.StartTime,
                MovieId = screeningCreate.MovieId,
                CinemaId = screeningCreate.CinemaId,
                RoomId = screeningCreate.RoomId
            };

            await _screeningRepository.Create(screening);
            await _screeningRepository.SaveChangeAsync();
        }

        public async Task Update(Guid id, ScreeningUpdateDTO screeningUpdateDTO)
        {
            var existedScreening = await _screeningRepository.GetScreeing(id);
            if(existedScreening == null) throw new Exception("Screening not found");

            if(screeningUpdateDTO.MovieId != Guid.Empty && IsMovieExisted(screeningUpdateDTO.MovieId))
                existedScreening.MovieId = screeningUpdateDTO.MovieId;

            if (screeningUpdateDTO.CinemaId != Guid.Empty && IsCinemaExisted(screeningUpdateDTO.CinemaId))
                existedScreening.CinemaId = screeningUpdateDTO.CinemaId;

            if (screeningUpdateDTO.RoomId != Guid.Empty && IsCinemaExisted(screeningUpdateDTO.RoomId))
                existedScreening.RoomId = screeningUpdateDTO.RoomId;

            existedScreening.StartTime = screeningUpdateDTO.StartTime;

            await _screeningRepository.SaveChangeAsync();
        }
        public async Task Delete(Guid id)
        {
            await _screeningRepository.Delete(id);
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

        #endregion
    }
}
