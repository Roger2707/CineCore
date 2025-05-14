using CinemaService.Data;
using CinemaService.Repositories.IRepositories;

namespace CinemaService.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CinemaDBContext _db;
        public UnitOfWork(CinemaDBContext db)
        {
            _db = db;

            Cinema = new CinemaRepository(_db);
            Room = new RoomRepository(_db);
            Seat = new SeatRepository(_db);
        }
        public ICinemaRepository Cinema { get; private set; }
        public IRoomRepository Room { get; private set; }
        public ISeatRepository Seat { get; private set; }


        #region Transactions Handlers

        public async Task BeginTransactionAsync()
        {
            await _db.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _db.Database.CommitTransactionAsync();
        }

        public async Task RollbackAsync()
        {
            await _db.Database.RollbackTransactionAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }

        #endregion
    }
}
