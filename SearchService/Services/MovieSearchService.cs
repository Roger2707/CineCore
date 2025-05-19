using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SearchService.Data;
using SearchService.DB;
using SearchService.Entities;

namespace SearchService.Services
{
    public class MovieSearchService
    {
        private readonly IMongoCollection<MovieSearch> _collection;
        private readonly GrpcMovieClientService _grpcMovieClientService;

        public MovieSearchService(IMongoDbContext context, IOptions<MongoDbSettings> settings, GrpcMovieClientService grpcMovieClientService)
        {
            _collection = context.GetCollection<MovieSearch>(settings.Value.MovieCollection);
            _grpcMovieClientService = grpcMovieClientService;
        }

        public async Task<List<MovieSearch>> GetAllMoviesAsync()
        {
            var movies = await _collection.Find(_ => true).ToListAsync();
            if(movies.Count > 0) return movies;

            // call gRPC from MovieService
            var gRpcMovies = _grpcMovieClientService.GetMovies();
            if (gRpcMovies == null || !gRpcMovies.Any())
                return [];

            var movieDocuments = gRpcMovies.Select(movie => new MovieSearch
            {
                Id = movie.Id,
                Title = movie.Title,
                DurationMinutes = movie.DurationMinutes,
                Description = movie.Description,
                PosterUrl = movie.PosterUrl,
                PublicId = movie.PublicId,
                Genres = movie.Genres
            }).ToList();

            // Insert Into MongoDB
            await _collection.InsertManyAsync(movieDocuments);

            return movieDocuments;
        }
    }
}
