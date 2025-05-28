using P1.MovieService.DTOs;
using P1.MovieService.Models;
using static Shared.Extensions.DynamicQueries.QueryableExtensions;
using static Shared.Extensions.DynamicQueries.QueryablePagedExtensions;

namespace P1.MovieService.Repositories.IRepositories
{
    public interface IMovieRepository
    {
        Task<List<MovieDTO>> GetAll();
        Task<PagedResult<Movie>> GetMovies(DynamicQueryRequest request, FilterConfiguration config = null);
        Task<MovieDTO> GetById(Guid id);
        Task<Movie> FirstOrDefaultAsync(Guid id);
        Task Create(Movie movie);
        Task Delete(Guid id);
        Task SaveChangeAsync();
    }
}
