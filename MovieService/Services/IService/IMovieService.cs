using MovieService.DTOs;
using MovieService.Models;

namespace MovieService.Services.IService
{
    public interface IMovieService
    {
        Task<List<MovieDTO>> GetAll(object param = null);
        Task<MovieDTO> GetById(Guid id);
        Task<Movie> FirstOrDefaultAsync(Guid id);
        Task CreateAsync(MovieCreateDTO movieDTO);
        Task UpdateAsync(Guid id,MovieUpdateDTO movieDTO);
        Task DeleteAsync(Guid id);
    }
}
