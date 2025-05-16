using Grpc.Core;
using CinemaService.Services.IServices;

namespace CinemaService.Services
{
    public class GrpcRoomService : GrpcRoom.GrpcRoomBase
    {
        private readonly IRoomService _roomService;
        public GrpcRoomService(IRoomService roomService)
        {
            _roomService = roomService;
        }
        public override async Task<GrpcRoomResponse> GetRoom(GetRoomRequest request, ServerCallContext context)
        {
            Console.WriteLine("---> Retrieve Grpc request for room");
            var room = await _roomService.GetRoom(Guid.Parse(request.Id))
                ?? throw new RpcException(new Status(StatusCode.NotFound, "Not Found"));

            var response = new GrpcRoomResponse()
            {
                Room = new GrpcRoomModel()
                {
                    Id = room.Id.ToString(),
                    Name = room.Name,
                    Cinema = room.Cinema,
                    TotalSeats = room.TotalSeats,
                }
            };
            return response;
        }
    }
}
