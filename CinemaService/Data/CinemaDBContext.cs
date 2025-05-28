using Microsoft.EntityFrameworkCore;
using P2.CinemaService.Models;

namespace P2.CinemaService.Data
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

            #region Delete Configurations
            modelBuilder.Entity<Screening>()
                .HasOne(s => s.Cinema)
                .WithMany()
                .HasForeignKey(s => s.CinemaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Screening>()
                .HasOne(s => s.Theater)
                .WithMany()
                .HasForeignKey(s => s.TheaterId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion

            modelBuilder.Entity<ScreeningSeat>()
                .HasKey(ss => new { ss.ScreeningId, ss.SeatId });
        }
    }
}
