using CinemaService.DTOs;
using CinemaService.Models;
using CinemaService.Repositories.IRepositories;
using CinemaService.Services.IServices;

namespace CinemaService.Services
{
    public class CinemaService : ICinemaService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CinemaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<Cinema>> GetAll()
        {
            return await _unitOfWork.Cinema.GetAll();
        }
        public async Task<Cinema> GetbyId(Guid id)
        {
            return await _unitOfWork.Cinema.GetbyId(id);
        }
        public async Task Create(CinemaCreateDTO cinemaCreateDTO)
        {
            var cinema = new Cinema
            {
                Name = cinemaCreateDTO.Name,
                Address = cinemaCreateDTO.Address,        
            };
            await _unitOfWork.Cinema.Create(cinema);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task Update(Guid id, CinemaUpdateDTO cinemaUpdateDTO)
        {
            var cinema = await _unitOfWork.Cinema.GetbyId(id);
            if(cinema == null) throw new ArgumentNullException("Cinema is not found !");

            cinema.Name = cinemaUpdateDTO.Name ?? cinema.Name;
            cinema.Address = cinemaUpdateDTO.Address ?? cinema.Address;

            await _unitOfWork.SaveChangesAsync();
        }
        public async Task Delete(Guid id)
        {
            await _unitOfWork.Cinema.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
