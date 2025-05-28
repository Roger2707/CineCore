using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MovieService.Data;
using MovieService.DTOs;
using MovieService.Models;
using MovieService.Repositories.IRepositories;
using static Shared.Extensions.DynamicQueries.QueryableExtensions;
using static Shared.Extensions.DynamicQueries.QueryablePagedExtensions;

namespace MovieService.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly MovieDBContext _db;
        public MovieRepository(MovieDBContext db)
        {
            _db = db;
        }

        #region CRUD

        public async Task Create(Movie movie)
        {
            await _db.Movies.AddAsync(movie);
        }

        public async Task Delete(Guid id)
        {
            var movie = await _db.Movies.FindAsync(id);
            _db.Remove(movie);
        }

        #endregion

        #region Retrieve
        public async Task<List<MovieDTO>> GetAll()
        {
            return await _db.Movies
                .Select(movie => new MovieDTO
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    Description = movie.Description,
                    DurationMinutes = movie.DurationMinutes,
                    PosterUrl = movie.PosterUrl,
                    PublicId = movie.PublicId,
                    Genres = movie.Genres.Select(g => Enum.GetName(typeof(Genre), g)).ToList()
                })
                .ToListAsync();
        }

        public async Task<Movie> FirstOrDefaultAsync(Guid id)
        {
            var movie = await _db.Movies.FirstOrDefaultAsync(m => m.Id == id);
            return movie;
        }

        public async Task<PagedResult<Movie>> GetMovies(DynamicQueryRequest request, FilterConfiguration config = null)
        {
            var query = _db.Movies.AsQueryable();
            if(request != null)
                query = query.DynamicQuery(request, config);

            var result = query.ToPagedResultAsync(
                page: (request.Skip / Math.Max(request.Take, 1)) + 1,
                pageSize: request.Take > 0 ? request.Take : 10
            );

            return result;
        }

        public async Task<MovieDTO> GetById(Guid id)
        {
            var movie = await _db.Movies
                .FromSqlRaw(" SELECT * FROM Movies WHERE Id = @Id ", new SqlParameter( "@Id", id))
                .FirstOrDefaultAsync();

            var movieDTO =  new MovieDTO
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                DurationMinutes = movie.DurationMinutes,
                PosterUrl = movie.PosterUrl,
                PublicId = movie.PublicId,
                Genres = movie.Genres.Select(g => Enum.GetName(typeof(Genre), g)).ToList()
            };

            return movieDTO;
        }

        #endregion

        public async Task SaveChangeAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
