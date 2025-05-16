using CinemaService;
using Grpc.Core;
using Grpc.Net.Client;
using ScheduleService.DTOs;

namespace ScheduleService.Services
{
    public class GrpcRoomClientService
    {
        private readonly ILogger<GrpcRoomClientService> _logger;
        private readonly IConfiguration _config;
        public GrpcRoomClientService(ILogger<GrpcRoomClientService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }
        public RoomDTO GetRoomById(Guid id)
        {
            var channel = GrpcChannel.ForAddress(_config["Grpc:GrpcRoom"], new GrpcChannelOptions
            {
                HttpHandler = new SocketsHttpHandler
                {
                    EnableMultipleHttp2Connections = true,
                },

                Credentials = ChannelCredentials.Insecure // use for http
            });

            var client = new GrpcRoom.GrpcRoomClient(channel);
            var request = new GetRoomRequest { Id = id.ToString() };

            try
            {
                var reply = client.GetRoom(request);
                var movie = new RoomDTO
                {
                    Id = Guid.Parse(reply.Room.Id),
                    Name = reply.Room.Name,
                    Cinema = reply.Room.Cinema,
                    TotalSeats = reply.Room.TotalSeats
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
