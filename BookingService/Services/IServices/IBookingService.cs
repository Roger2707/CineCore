using BookingService.DTOs;

namespace BookingService.Services.IServices
{
    public interface IBookingService
    {
        Task<List<BookingDTO>> GetBookings();
        Task<BookingDTO> GetBooking(Guid bookingId);
        Task Create(BookingCreateRequestDTO request);
    }
}
