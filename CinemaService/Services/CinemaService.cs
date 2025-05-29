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
        public async Task<CinemaBaseDTO> GetbyId(Guid id)
        {
            var cinema = await _unitOfWork.Cinema.GetbyId(id);

            if (_currentUserService.IsCustomer)
                return new CinemaBaseDTO
                {
                    Id = cinema.Id,
                    Name = cinema.Name,
                    Address = cinema.Address,
                };

            if (_currentUserService.IsAdmin)
            {
                if (!_currentUserService.CinemaId.HasValue)
                    throw new UnauthorizedAccessException("You are not authorized to access this cinema.");

                else if (_currentUserService.CinemaId != id)
                {
                    return new CinemaBaseDTO
                    {
                        Id = cinema.Id,
                        Name = cinema.Name,
                        Address = cinema.Address,
                    };
                }
            }

            // case : user is admin and has access to the cinema
            return new CinemaExtraDTO
            {
                Id = cinema.Id,
                Name = cinema.Name,
                Address = cinema.Address,
                Created = cinema.Created
            };
        }

        public async Task Create(CinemaCreateDTO cinemaCreateDTO)
        {
            if(!_currentUserService.IsSuperAdmin)
                throw new UnauthorizedAccessException("You are not authorized to create a cinema.");

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
            if (_currentUserService.IsCustomer)
                throw new UnauthorizedAccessException("You are not authorized to create a cinema.");

            if (_currentUserService.IsAdmin)
            {
                if (!_currentUserService.CinemaId.HasValue || _currentUserService.CinemaId != id)
                    throw new UnauthorizedAccessException("You are not authorized to access this cinema.");
            }

            var cinema = await _unitOfWork.Cinema.GetbyId(id);
            if(cinema == null) throw new ArgumentNullException("Cinema is not found !");

            cinema.Name = cinemaUpdateDTO.Name ?? cinema.Name;
            cinema.Address = cinemaUpdateDTO.Address ?? cinema.Address;

            await _unitOfWork.SaveChangesAsync();
        }
        public async Task Delete(Guid id)
        {
            if (!_currentUserService.IsSuperAdmin)
                throw new UnauthorizedAccessException("You are not authorized to create a cinema.");

            await _unitOfWork.Cinema.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
