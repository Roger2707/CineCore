using MovieService.Data;
using Grpc.Core;

namespace MovieService.Services
{
    public class GrpcMovieService : GrpcMovie
        private readonly MovieDBContext _db;

        public GrpcMovieService(MovieDBContext db)
        {
            _db = db;
        }
    }
}
