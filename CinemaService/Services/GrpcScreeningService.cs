using P2.CinemaService.Services.IServices;
using Grpc.Core;

namespace P2.CinemaService.Services
{
    public class GrpcScreeningService : GrpcScreening.GrpcScreeningBase
    {
        private readonly IScreeingService _screeingServicee;
        public GrpcScreeningService(IScreeingService screeingService)
        {
            _screeingServicee = screeingService;
        }

        public override async Task<GrpcScreeningResponse> GetScreening(GetScreeningRequest request, ServerCallContext context)
        {
            Console.WriteLine("---> Retrieve Grpc request for Screening");

            var cinema = await _screeingServicee.GetbyId(Guid.Parse(request.Id))
                ?? throw new RpcException(new Status(StatusCode.NotFound, "Not Found"));

            var response = new GrpcScreeningResponse()
            {
                Screening = new GrpcScreeningModel()
                {
                    Id = cinema.Id.ToString(),
                }
            };
            return response;
        }
    }
}
