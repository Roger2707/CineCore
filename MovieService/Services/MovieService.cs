using Contracts;
using MassTransit;
using MovieService.DTOs;
using MovieService.Models;
using MovieService.Repositories.IRepositories;
using MovieService.Services.IService;

namespace MovieService.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly CloudinaryService _cloudinaryService;
        private readonly IPublishEndpoint _publishEndpoint;
        public MovieService(IMovieRepository movieRepository, CloudinaryService cloudinaryService, IPublishEndpoint publishEndpoint)
        {
            _movieRepository = movieRepository;
            _cloudinaryService = cloudinaryService;
            _publishEndpoint = publishEndpoint;
        }
        public async Task<List<MovieDTO>> GetAll(object param = null)
        {
            return await _movieRepository.GetAll(param);
        }

        public async Task<Movie> FirstOrDefaultAsync(Guid id)
        {
            return await _movieRepository.FirstOrDefaultAsync(id);
        }

        public async Task<MovieDTO> GetById(Guid id)
        {
            return await _movieRepository.GetById(id);
        }

        public async Task CreateAsync(MovieCreateDTO movieDTO)
        {
            var movie = new Movie
            {
                Title = movieDTO.Title,
                Description = movieDTO.Description,
                DurationMinutes = movieDTO.DurationMinutes,
                Genres = movieDTO.Genres
            };

            if(movieDTO.PosterFile != null)
            {
                var uploadResult = await _cloudinaryService.AddImageAsync(movieDTO.PosterFile, "movies");

                if (uploadResult.Error != null)
                    throw new ArgumentException(uploadResult.Error.Message);

                movie.PosterUrl = uploadResult.SecureUrl.AbsoluteUri;
                movie.PublicId = uploadResult.PublicId;
            }

            await _movieRepository.Create(movie);

            await _publishEndpoint.Publish(new MovieCreated
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                DurationMinutes = movie.DurationMinutes,
                PosterUrl = movie.PosterUrl,
                PublicId = movie.PublicId,
                Genres = movie.Genres.Select(g => Enum.GetName(typeof(Genre), g)).ToList()
            });

            await _movieRepository.SaveChangeAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _movieRepository.Delete(id);
            await _movieRepository.SaveChangeAsync();
        }

        public async Task UpdateAsync(Guid id, MovieUpdateDTO movieDTO)
        {
            var movie = await _movieRepository.FirstOrDefaultAsync(id);

            movie.Title = (movieDTO.Title == movie.Title || string.IsNullOrEmpty(movieDTO.Title)) ? movie.Title : movieDTO.Title;
            movie.Description = (movieDTO.Description == movie.Description || string.IsNullOrEmpty(movieDTO.Description)) ? movie.Description : movieDTO.Description;
            movie.DurationMinutes = (movieDTO.DurationMinutes == movie.DurationMinutes || movieDTO.DurationMinutes > 0) ? movie.DurationMinutes : movieDTO.DurationMinutes;
            movie.Genres = movieDTO.Genres;

            if (movieDTO.PosterFile != null)
            {
                var uploadResult = await _cloudinaryService.AddImageAsync(movieDTO.PosterFile, "movies");

                if (uploadResult.Error != null)
                    throw new ArgumentException(uploadResult.Error.Message);

                if (!string.IsNullOrEmpty(movie.PublicId))
                    await _cloudinaryService.DeleteImageAsync(movie.PublicId);

                movie.PosterUrl = uploadResult.SecureUrl.AbsoluteUri;
                movie.PublicId = uploadResult.PublicId;
            }

            await _movieRepository.SaveChangeAsync();
        }
    }
}
