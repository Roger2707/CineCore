using BookingService.Data;
using BookingService.DTOs;
using BookingService.Models;
using BookingService.Repositories.IRepositories;
using Contracts.BookingEvents;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BookingService.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BookingDbContext _db;
        private readonly IPublishEndpoint _publishEndpoint;
        private IDbContextTransaction _transaction;

        public BookingRepository(BookingDbContext db, IPublishEndpoint publishEndpoint)
        {
            _db = db;
            _publishEndpoint = publishEndpoint;
        }

        #region Create / Delete

        public async Task Create(Booking booking)
        {
            await _db.AddAsync(booking);
        }

        public async Task Create(List<BookingSeat> bookingSeats)
        {
            await _db.AddRangeAsync(bookingSeats);
        }

        public async Task Delete(Guid bookingId)
        {
            var booking = await _db.Bookings.FindAsync(bookingId);
            if (booking != null)
            {
                _db.Bookings.Remove(booking);
            }
        }

        #endregion

        #region Retrieve Data

        public async Task<BookingDTO> GetBooking(Guid bookingId)
        { 
            string query = @" 
                            SELECT b.Id, b.UserId, b.ScreeningId, b.TotalPrice, b.Created, b.BookingStatus, bs.Id as BôkingSeatId, bs.SeatId, bs.Price, '' as SeatInfo
                            FROM Bookings b 
                            INNER JOIN BookingSeats bs ON bs.BookingId = b.Id  
                            WHERE Id = @BookingId
                            ";
            var parameters = new { BookingId = bookingId };

            var booking = await _db.Database.SqlQueryRaw<BookingRetrieveRow>(query, parameters).ToListAsync();
            if (booking == null || booking.Count == 0) return null;

            return booking
                .GroupBy(b => new { b.Id, b.UserId, b.ScreeningId, b.BookingStatus, b.TotalPrice, b.Created})
                .Select(g => new BookingDTO
                {
                    Id = g.Key.Id,
                    UserId = g.Key.UserId,
                    ScreeningId = g.Key.ScreeningId,
                    BookingStatus = Enum.GetName(typeof(BookingStatus), Int32.Parse(g.Key.BookingStatus)),
                    TotalPrice = g.Key.TotalPrice,
                    Created = g.Key.Created,
                    Items = g.Select(b => new BookingSeatDTO
                    {
                        Id = b.BookingSeatId,
                        SeatId = b.SeatId,
                        Price = b.Price
                    }).ToList()
                })
                .FirstOrDefault();
        }

        public async Task<List<BookingDTO>> GetBookings()
        {
            string query = @" 
                            SELECT b.Id, b.UserId, b.ScreeningId, b.TotalPrice, b.Created, b.BookingStatus, bs.Id as BôkingSeatId, bs.SeatId, bs.Price, '' as SeatInfo
                            FROM Bookings b 
                            INNER JOIN BookingSeats bs ON bs.BookingId = b.Id  
                            ";

            var booking = await _db.Database.SqlQueryRaw<BookingRetrieveRow>(query).ToListAsync();
            if (booking == null || booking.Count == 0) return null;

            return booking
                .GroupBy(b => new { b.Id, b.UserId, b.ScreeningId, b.BookingStatus, b.TotalPrice, b.Created })
                .Select(g => new BookingDTO
                {
                    Id = g.Key.Id,
                    UserId = g.Key.UserId,
                    ScreeningId = g.Key.ScreeningId,
                    BookingStatus = Enum.GetName(typeof(BookingStatus), Int32.Parse(g.Key.BookingStatus)),
                    TotalPrice = g.Key.TotalPrice,
                    Created = g.Key.Created,
                    Items = g.Select(b => new BookingSeatDTO
                    {
                        Id = b.BookingSeatId,
                        SeatId = b.SeatId,
                        Price = b.Price
                    }).ToList()
                })
                .ToList();
        }

        #endregion

        #region Transactions
        public async Task BeginTransactionAsync()
        {
            _transaction = await _db.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _transaction?.CommitAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _transaction?.RollbackAsync();
        }

        public async Task SaveChangeAsync()
        {
            await _db.SaveChangesAsync();
        }

        public async Task PublishMessageAsync<T>(T message) where T : class
        {
            await _publishEndpoint.Publish(message);
        }

        #endregion
    }
}
