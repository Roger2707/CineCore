using CinemaService.DTOs;
using Grpc.Core;
using Grpc.Net.Client;
using MovieService;

namespace CinemaService.Services
{
    public class GrpcMovieClientService
    {
        private readonly ILogger<GrpcMovieClientService> _logger;
        private readonly IConfiguration _config;

        public GrpcMovieClientService(ILogger<GrpcMovieClientService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public MovieDTO GetMovieById(Guid id)
        {
            var channel = GrpcChannel.ForAddress(_config["Grpc:GrpcMovie"], new GrpcChannelOptions
            {
                HttpHandler = new SocketsHttpHandler
                {
                    EnableMultipleHttp2Connections = true,
                },

                Credentials = ChannelCredentials.Insecure // use for http
            });

            var client = new GrpcMovie.GrpcMovieClient(channel);
            var request = new GetMovieRequest { Id = id.ToString() };

            try
            {
                var reply = client.GetMovie(request);
                var movie = new MovieDTO
                {
                    Id = Guid.Parse(reply.Movie.Id),
                    Title = reply.Movie.Title,
                    DurationMinutes = reply.Movie.DurationMinutes,
                    Description = reply.Movie.Description,
                    PosterUrl = reply.Movie.PosterUrl,
                    PublicId = reply.Movie.PublicId,
                    Genres = reply.Movie.Genres?.ToList()
                };
                return movie;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "=> Error when call Grpc service");
                return null;
            }
        }

        public List<MovieDTO> GetMovies()
        {
            var channel = GrpcChannel.ForAddress(_config["Grpc:GrpcMovie"], new GrpcChannelOptions
            {
                HttpHandler = new SocketsHttpHandler
                {
                    EnableMultipleHttp2Connections = true,
                },

                Credentials = ChannelCredentials.Insecure // use for http
            });

            var client = new GrpcMovie.GrpcMovieClient(channel);
            var request = new GetAllMoviesRequest();

            try
            {
                var reply = client.GetAllMovies(request);
                var movies = reply.Movies.Select(m => new MovieDTO
                {
                    Id = Guid.Parse(m.Id),
                    Title = m.Title,
                    DurationMinutes = m.DurationMinutes,
                    Description = m.Description,
                    PosterUrl = m.PosterUrl,
                    PublicId = m.PublicId,
                    Genres = m.Genres?.ToList()
                }).ToList();
                return movies;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "=> Error when call Grpc service");
                return null;
            }
        }
    }
}
