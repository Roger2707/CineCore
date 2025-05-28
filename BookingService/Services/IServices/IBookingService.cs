using P4.BookingService.DTOs;

namespace P4.BookingService.Services.IServices
{
    public interface IBookingService
    {
        Task<List<BookingDTO>> GetBookings();
        Task<BookingDTO> GetBooking(Guid bookingId);
        Task Create(BookingCreateRequestDTO request);
        Task Delete(Guid bookingId, List<Guid> SeatIds, Guid ScreeningId);
        Task SaveChangeAsync();
    }
}
