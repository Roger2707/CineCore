using Microsoft.EntityFrameworkCore;
using MovieService.Data;
using MovieService.DTOs;
using MovieService.Models;
using MovieService.Repositories.IRepositories;

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

        public async Task<Movie> FirstOrDefaultAsync(Guid id)
        {
            var movie = await _db.Movies.FirstOrDefaultAsync(m => m.Id == id);
            return movie;
        }

        public async Task<List<MovieDTO>> GetAll(object param = null)
        {
            var movies = await _db.Movies
                .FromSqlRaw(" SELECT * FROM Movies ", param)
                .ToListAsync();

            var movieDTOs = movies.Select(m => new MovieDTO
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                DurationMinutes = m.DurationMinutes,
                PosterUrl = m.PosterUrl,
                PublicId = m.PublicId,
                Genres = m.Genres.Select(g => Enum.GetName(typeof(Genre), g)).ToList()
            }).ToList();

            return movieDTOs;
        }

        public async Task<MovieDTO> GetById(Guid id)
        {
            var movie = await _db.Movies
                .FromSqlRaw(" SELECT * FROM Movies WHERE Id = @Id ", id)
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
