using BookingService.DTOs;
using BookingService.Models;

namespace BookingService.Repositories.IRepositories
{
    public interface IBookingRepository
    {
        Task<List<BookingDTO>> GetBookings();
        Task<BookingDTO> GetBooking(Guid bookingId);
        Task Create(Booking booking);
        Task Create(List<BookingSeat> bookingSeats);
        Task Delete(Guid bookingId);
        Task SaveChangeAsync();
    }
}
