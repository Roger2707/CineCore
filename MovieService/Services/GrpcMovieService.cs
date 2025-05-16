using Grpc.Core;
using MovieService.Services.IService;

namespace MovieService.Services
{
    public class GrpcMovieService : GrpcMovie.GrpcMovieBase
    {
        private readonly IMovieService _movieService;
        public GrpcMovieService(IMovieService movieService)
        {
            _movieService = movieService;
        }

        public override async Task<GrpcMovieResponse> GetMovie(GetMovieRequest request, ServerCallContext context)
        {
            Console.WriteLine("---> Retrieve Grpc request for movie");
            var movie = await _movieService.GetById(Guid.Parse(request.Id)) 
                ?? throw new RpcException(new Status(StatusCode.NotFound, "Not Found"));

            var response = new GrpcMovieResponse()
            {
                Movie = new GrpcMovieModel()
                {
                    Id = movie.Id.ToString(),
                    Title = movie.Title,
                    Description = movie.Description,
                    DurationMinutes = movie.DurationMinutes,
                    PosterUrl = movie.PosterUrl,
                    PublicId = movie.PublicId,
                }            
            };
            response.Movie.Genres.AddRange(movie.Genres);
            return response;
        }
    }
}
