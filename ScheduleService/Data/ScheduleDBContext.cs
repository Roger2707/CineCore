using Microsoft.EntityFrameworkCore;
using ScheduleService.Models;

namespace ScheduleService.Data
{
    public class ScheduleDBContext : DbContext
    {
        public ScheduleDBContext(DbContextOptions<ScheduleDBContext> options) : base(options)
        {
        }
        public DbSet<Screening> Screenings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
