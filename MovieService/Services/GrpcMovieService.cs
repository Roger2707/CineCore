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

        public override async Task<GrpcMoviesResponse> GetAllMovies(GetAllMoviesRequest request, ServerCallContext context)
        {
            Console.WriteLine("---> Retrieve all movies - gRPC");
            var movies = await _movieService.GetAll();
            var moviesResponse = movies.Select(m => new GrpcMovieModel()
            {
                Id = m.Id.ToString(),
                Title = m.Title,
                Description = m.Description,
                DurationMinutes = m.DurationMinutes,
                PosterUrl = m.PosterUrl,
                PublicId = m.PublicId,
                Genres = { m.Genres }
            }).ToList();

            var response = new GrpcMoviesResponse();
            response.Movies.AddRange(moviesResponse);
            return response;
        }
    }
}
