namespace CinemaService.Repositories.IRepositories
{
    public interface IUnitOfWork
    {
        Task BeginTransactionAsync();
        Task SaveChangesAsync();
        Task CommitAsync();
        Task RollbackAsync();
        ICinemaRepository Cinema { get; }
        IRoomRepository Room { get; }
        ISeatRepository Seat { get; }
    }
}
