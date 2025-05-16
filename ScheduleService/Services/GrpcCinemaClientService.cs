using CinemaService;
using Grpc.Core;
using Grpc.Net.Client;
using ScheduleService.DTOs;

namespace ScheduleService.Services
{
    public class GrpcCinemaClientService
    {
        private readonly ILogger<GrpcCinemaClientService> _logger;
        private readonly IConfiguration _config;

        public GrpcCinemaClientService(ILogger<GrpcCinemaClientService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public CinemaDTO GetCinemaById(Guid id)
        {
            var channel = GrpcChannel.ForAddress(_config["Grpc:GrpcCinema"], new GrpcChannelOptions
            {
                HttpHandler = new SocketsHttpHandler
                {
                    EnableMultipleHttp2Connections = true,
                },

                Credentials = ChannelCredentials.Insecure // use for http
            });

            var client = new GrpcCinema.GrpcCinemaClient(channel);
            var request = new GetCinemaRequest { Id = id.ToString() };

            try
            {
                var reply = client.GetCinema(request);
                var cinema = new CinemaDTO
                {
                    Id = Guid.Parse(reply.Cinema.Id),
                    Name = reply.Cinema.Name,
                    Address = reply.Cinema.Address,
                };
                return cinema;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "=> Error when call Grpc service");
                return null;
            }
        }
    }
}
