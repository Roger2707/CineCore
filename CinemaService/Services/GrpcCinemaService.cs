using CinemaService.Services.IServices;
using Grpc.Core;

namespace CinemaService.Services
{
    public class GrpcCinemaService : GrpcCinema.GrpcCinemaBase
    {
        private readonly ICinemaService _cinemaService;
        public GrpcCinemaService(ICinemaService cinemaService)
        {
            _cinemaService = cinemaService;
        }

        public override async Task<GrpcCinemaResponse> GetCinema(GetCinemaRequest request, ServerCallContext context)
        {
            Console.WriteLine("---> Retrieve Grpc request for cinema");

            var cinema = await _cinemaService.GetbyId(Guid.Parse(request.Id))
                ?? throw new RpcException(new Status(StatusCode.NotFound, "Not Found"));

            var response = new GrpcCinemaResponse()
            {
                Cinema = new GrpcCinemaModel()
                {
                    Id = cinema.Id.ToString(),
                    Name = cinema.Name,
                    Address = cinema.Address,
                }
            };
            return response;
        }
    }
}
