using ScheduleService.DTOs;
using ScheduleService.Models;
using ScheduleService.Repositories.IRepositories;
using ScheduleService.Services.IServices;

namespace ScheduleService.Services
{
    public class ScreeningService : IScreeningService
    {
        private readonly IScreeningRepository _screeningRepository;
        private readonly GrpcMovieClientService _grpcMovieClientService;
        private readonly GrpcCinemaClientService _grpcCinemaClientService;
        private readonly GrpcRoomClientService _grpcRoomClientService;

        public ScreeningService(IScreeningRepository screeningRepository, GrpcMovieClientService grpcMovieClientService,
                GrpcCinemaClientService grpcCinemaClientService, GrpcRoomClientService grpcRoomClientService
            )
        {
            _screeningRepository = screeningRepository;
            _grpcMovieClientService = grpcMovieClientService;
            _grpcCinemaClientService = grpcCinemaClientService;
            _grpcRoomClientService = grpcRoomClientService;
        }

        #region Validations

        private bool IsMovieExisted(Guid movieId)
        {
            var movie = _grpcMovieClientService.GetMovieById(movieId);
            return movie != null;
        }

        private bool IsCinemaExisted(Guid cinemaId)
        {
            var cinema = _grpcCinemaClientService.GetCinemaById(cinemaId);
            return cinema != null;
        }

        private bool IsRoomExisted(Guid roomId)
        {
            var room = _grpcRoomClientService.GetRoomById(roomId);
            return room != null;
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

        public async Task<List<ScreeningDTO>> GetAll()
        {
            var screenings = await _screeningRepository.GetAll();
            return screenings.Select(s => new ScreeningDTO
            {
                Id = s.Id,
                StartTime = s.StartTime,
                MovieId = s.MovieId,
                Title = _grpcMovieClientService.GetMovieById(s.MovieId).Title,
                Description = _grpcMovieClientService.GetMovieById(s.MovieId).Description,
                DurationMinutes = _grpcMovieClientService.GetMovieById(s.MovieId).DurationMinutes,
                PosterUrl = _grpcMovieClientService.GetMovieById(s.MovieId).PosterUrl,
                PublicId = _grpcMovieClientService.GetMovieById(s.MovieId).PublicId,
                Genres = _grpcMovieClientService.GetMovieById(s.MovieId).Genres,

                CinemaId = s.CinemaId,
                CinemaName = _grpcCinemaClientService.GetCinemaById(s.CinemaId).Name,
                CinemaAddress = _grpcCinemaClientService.GetCinemaById(s.CinemaId).Address,

                RoomId = s.RoomId,
                RoomName = _grpcRoomClientService.GetRoomById(s.RoomId).Name,
                NumberOfSeats = _grpcRoomClientService.GetRoomById(s.RoomId).TotalSeats
            }).ToList();
        }

        public async Task<ScreeningDTO> GetById(Guid id)
        {
            var screening = await _screeningRepository.GetById(id);

            var movie = _grpcMovieClientService.GetMovieById(screening.MovieId);
            var cinema = _grpcCinemaClientService.GetCinemaById(screening.CinemaId);
            var room = _grpcRoomClientService.GetRoomById(screening.RoomId);

            return new ScreeningDTO
            {
                Id = screening.Id,
                StartTime = screening.StartTime,

                MovieId = screening.MovieId,
                Title = movie.Title,
                Description = movie.Description,
                DurationMinutes = movie.DurationMinutes,
                PosterUrl = movie.PosterUrl,
                PublicId = movie.PublicId,
                Genres = movie.Genres,

                CinemaId = screening.CinemaId,
                CinemaName = cinema.Name,
                CinemaAddress = cinema.Address,

                RoomId = screening.RoomId,
                RoomName = room.Name,
                NumberOfSeats = room.TotalSeats
            };
        }

        #endregion
    }
}
