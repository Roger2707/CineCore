using CinemaService.Data;
using CinemaService.Models;
using CinemaService.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace CinemaService.Repositories
{
    public class CinemaRepository : ICinemaRepository
    {
        private readonly CinemaDBContext _db;
        public CinemaRepository(CinemaDBContext db)
        {
            _db = db;
        }
        public async Task<List<Cinema>> GetAll()
        {
            var cinemas = await _db.Cinemas.ToListAsync();
            return cinemas;
        }

        public async Task<Cinema> GetbyId(Guid id)
        {
            var cinama = await _db.Cinemas.FirstOrDefaultAsync(x => x.Id == id);
            return cinama;
        }
        public async Task Create(Cinema cinema)
        {
            await _db.Cinemas.AddAsync(cinema);
        }

        public async Task Delete(Guid id)
        {
            var cinema = await _db.Cinemas.FirstOrDefaultAsync(x => x.Id == id);
            if (cinema == null) throw new ArgumentNullException("Cinema is not found !");

            _db.Cinemas.Remove(cinema);
        }
    }
}
