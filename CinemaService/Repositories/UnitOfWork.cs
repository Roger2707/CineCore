using CinemaService.Data;
using CinemaService.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace CinemaService.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CinemaDBContext _db;
        private IDbContextTransaction _transaction;

        public UnitOfWork(CinemaDBContext db)
        {
            _db = db;

            Cinema = new CinemaRepository(_db);
            Theater = new TheaterRepository(_db);
            Seat = new SeatRepository(_db);
            Screening = new ScreeningRepository(_db);
        }
        public ICinemaRepository Cinema { get; private set; }
        public ITheaterRepository Theater { get; private set; }
        public ISeatRepository Seat { get; private set; }
        public IScreeningRepository Screening { get; private set; }


        #region Transactions Handlers

        public async Task BeginTransactionAsync()
        {
            _transaction = await _db.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _transaction?.CommitAsync();
        }

        public async Task RollbackAsync()
        {
            await _transaction?.RollbackAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }

        #endregion
    }
}
