using Grpc.Core;
using Grpc.Net.Client;
using MovieService;
using ScheduleService.DTOs;

namespace ScheduleService.Services
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
    }
}
