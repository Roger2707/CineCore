using BookingService.Data;
using BookingService.DTOs;
using BookingService.Models;
using BookingService.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BookingDbContext _db;

        public BookingRepository(BookingDbContext db)
        {
            _db = db;
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

        public async Task SaveChangeAsync()
        {
            await _db.SaveChangesAsync();
        }

        #endregion
    }
}
