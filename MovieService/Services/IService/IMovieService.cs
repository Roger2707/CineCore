using P1.MovieService.DTOs;
using P1.MovieService.Models;
using static Shared.Extensions.DynamicQueries.QueryableExtensions;
using static Shared.Extensions.DynamicQueries.QueryablePagedExtensions;

namespace P1.MovieService.Services.IService
{
    public interface IMovieService
    {
        Task<List<MovieDTO>> GetAll();
        Task<PagedResult<MovieDTO>> GetAll(DynamicQueryRequest request = null, FilterConfiguration config = null);
        Task<MovieDTO> GetById(Guid id);
        Task<Movie> FirstOrDefaultAsync(Guid id);
        Task CreateAsync(MovieCreateDTO movieDTO);
        Task UpdateAsync(Guid id,MovieUpdateDTO movieDTO);
        Task DeleteAsync(Guid id);
    }
}
