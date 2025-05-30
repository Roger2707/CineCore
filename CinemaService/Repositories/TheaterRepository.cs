﻿using Microsoft.EntityFrameworkCore;
using P2.CinemaService.Repositories.IRepositories;
using P2.CinemaService.Data;
using P2.CinemaService.Models;

namespace P2.CinemaService.Repositories
{
    public class TheaterRepository : ITheaterRepository
    {
        private readonly CinemaDBContext _db;
        public TheaterRepository(CinemaDBContext db)
        {
            _db = db;
        }

        public async Task Create(Theater room)
        {
           await _db.Theaters.AddAsync(room);
        }

        public async Task Delete(Guid id)
        {
            var room = await _db.Theaters.FirstOrDefaultAsync(x => x.Id == id);
            _db.Theaters.Remove(room);
        }

        public async Task<List<Theater>> GetAll(Guid cinemaId)
        {
            var theaters = await _db.Theaters.Include(t => t.Cinema).ToListAsync();
            return theaters;
        }

        public async Task<Theater> GetbyId(Guid id)
        {
            var theater = await _db.Theaters.Include(t => t.Cinema).FirstOrDefaultAsync();
            return theater;
        }
    }
}
