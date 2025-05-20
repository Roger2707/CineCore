using Grpc.Core;
using ScheduleService.Services.IServices;

namespace ScheduleService.Services
{
    public class GrpcScreeningService : GrpcScreening.GrpcScreeningBase
    {
        private readonly IScreeningService _screeningService;
        public GrpcScreeningService(IScreeningService screeningService)
        {
            _screeningService = screeningService;
        }

        public override async Task<GrpcScreeningResponse> GetScreening(GetScreeningRequest request, ServerCallContext context)
        {
            Console.WriteLine("---> Retrieve Grpc request for Screening");
            var movie = await _screeningService.GetById(Guid.Parse(request.Id))
                ?? throw new RpcException(new Status(StatusCode.NotFound, "Not Found"));

            var response = new GrpcScreeningResponse()
            {
                Screening = new GrpcScreeningModel()
                {
                    Id = movie.Id.ToString(),
                }
            };
            return response;
        }
    }
}
