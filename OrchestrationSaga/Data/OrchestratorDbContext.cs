using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrchestrationSaga.StateMachine;

namespace OrchestrationSaga.Data
{
    public class OrchestratorDbContext : SagaDbContext
    {
        public OrchestratorDbContext(DbContextOptions options) : base(options) { }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new BookingStateMap(); }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

    public class BookingStateMap : SagaClassMap<BookingState>
    {
        protected override void Configure(EntityTypeBuilder<BookingState> entity, ModelBuilder model)
        {
            entity.HasKey(e => e.CorrelationId);
            entity.Property(e => e.CurrentState).HasMaxLength(64);
            entity.Property(e => e.BookingId); 
            entity.Property(e => e.SeatIds);
            entity.Property(e => e.ScreeningId);
            entity.Property(e => e.UserId);
            entity.Property(e => e.PaymentIntentId).HasMaxLength(128);
            entity.ToTable("BookingStates");
        }
    }
}
