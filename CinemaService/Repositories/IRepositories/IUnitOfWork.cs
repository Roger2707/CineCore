namespace CinemaService.Repositories.IRepositories
{
    public interface IUnitOfWork
    {
        Task BeginTransactionAsync();
        Task SaveChangesAsync();
        Task CommitAsync();
        Task RollbackAsync();

        ICinemaRepository Cinema { get; }
        ITheaterRepository Theater { get; }
        ISeatRepository Seat { get; }
        IScreeningRepository Screening { get; }
    }
}
