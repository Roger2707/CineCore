using BookingService.DTOs;
using Grpc.Core;
using Grpc.Net.Client;
using ScheduleService;

namespace BookingService.Services
{
    public class GrpcScreeningClientService
    {
        private readonly ILogger<GrpcScreeningClientService> _logger;
        private readonly IConfiguration _config;
        public GrpcScreeningClientService(ILogger<GrpcScreeningClientService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }
        public ScreeningDTOResponse GetScreening(Guid id)
        {
            var channel = GrpcChannel.ForAddress(_config["Grpc:GrpcScreening"], new GrpcChannelOptions
            {
                HttpHandler = new SocketsHttpHandler
                {
                    EnableMultipleHttp2Connections = true,
                },

                Credentials = ChannelCredentials.Insecure // use for http
            });

            var client = new GrpcScreening.GrpcScreeningClient(channel);
            var request = new GetScreeningRequest { Id = id.ToString() };

            try
            {
                var reply = client.GetScreening(request);
                var screeing = new ScreeningDTOResponse
                {
                    Id = Guid.Parse(reply.Screening.Id),
                };
                return screeing;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "=> Error when call Grpc service");
                return null;
            }
        }
    }
}
