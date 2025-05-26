using CinemaService.Models;
using Microsoft.EntityFrameworkCore;
using static Grpc.Core.Metadata;

namespace CinemaService.Data
{
    public class CinemaDBContext : DbContext
    {
        public CinemaDBContext(DbContextOptions<CinemaDBContext> options) : base(options)
        {

        }

        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Theater> Theaters { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Screening> Screenings { get; set; }
        public DbSet<ScreeningSeat> ScreeningSeats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ScreeningSeat>()
                .HasKey(ss => new { ss.ScreeningId, ss.SeatId });
        }
    }
}
