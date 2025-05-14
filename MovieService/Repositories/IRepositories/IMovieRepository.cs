using MovieService.DTOs;
using MovieService.Models;

namespace MovieService.Repositories.IRepositories
{
    public interface IMovieRepository
    {
        Task<List<MovieDTO>> GetAll(object param = null);
        Task<MovieDTO> GetById(Guid id);
        Task<Movie> FirstOrDefaultAsync(Guid id);
        Task Create(Movie movie);
        Task Delete(Guid id);
        Task SaveChangeAsync();
    }
}
