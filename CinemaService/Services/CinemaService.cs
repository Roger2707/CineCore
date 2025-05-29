using P2.CinemaService.DTOs;
using P2.CinemaService.Models;
using P2.CinemaService.Repositories.IRepositories;
using P2.CinemaService.Services.IServices;
using Shared.Services.IServices;

namespace P2.CinemaService.Services
{
    public class CinemaService : ICinemaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public CinemaService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }
        public async Task<List<Cinema>> GetAll()
        {
            return await _unitOfWork.Cinema.GetAll();
        }
        public async Task<Cinema> GetbyId(Guid id)
        {
            if(_currentUserService.IsCustomer) 
                throw new UnauthorizedAccessException("You are not authorized to access this resource.");

            if (_currentUserService.IsAdmin)
            {
                if (!_currentUserService.CinemaId.HasValue || _currentUserService.CinemaId != id)
                {
                    throw new UnauthorizedAccessException("You are not authorized to access this cinema.");
                }
            }       
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
