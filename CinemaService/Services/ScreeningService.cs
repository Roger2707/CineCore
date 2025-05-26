using CinemaService.DTOs;
using CinemaService.Models;
using CinemaService.Repositories.IRepositories;
using CinemaService.Services.IServices;

namespace CinemaService.Services
{
    public class ScreeningService : IScreeingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ScreeningService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Create(ScreeningCreateDTO screeningCreateDTO)
        {
            var screening = new Screening
            {
                MovieId = screeningCreateDTO.MovieId,
                CinemaId = screeningCreateDTO.CinemaId,
                TheaterId = screeningCreateDTO.TheaterId,
                StartTime = screeningCreateDTO.StartTime,
                EndTime = screeningCreateDTO.EndTime,
                Duration = (int)(screeningCreateDTO.EndTime - screeningCreateDTO.StartTime).TotalMinutes
            };
            await _unitOfWork.Screening.Create(screening);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            await _unitOfWork.Screening.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Update(Guid id, ScreeningUpdateDTO screeningUpdateDTO)
        {
            var existedScreening = await _unitOfWork.Screening.GetById(id);
            if(existedScreening == null) throw new Exception("Screening not found");

            existedScreening.MovieId = screeningUpdateDTO.MovieId ?? existedScreening.MovieId;
            existedScreening.CinemaId = screeningUpdateDTO.CinemaId ?? existedScreening.CinemaId;
            existedScreening.TheaterId = screeningUpdateDTO.TheaterId ?? existedScreening.TheaterId;
            existedScreening.StartTime = screeningUpdateDTO.StartTime ?? existedScreening.StartTime;
            existedScreening.EndTime = screeningUpdateDTO.EndTime ?? existedScreening.EndTime;
            existedScreening.Duration = (int)(existedScreening.EndTime - existedScreening.StartTime).TotalMinutes;

            await _unitOfWork.SaveChangesAsync();
        }

        #region Retrived

        public async Task<List<Screening>> GetAll(Guid cinemaId)
        {
            return await _unitOfWork.Screening.GetAll(cinemaId);
        }

        public async Task<Screening> GetbyId(Guid id)
        {
            return await _unitOfWork.Screening.GetById(id);
        }

        #endregion
    }
}
