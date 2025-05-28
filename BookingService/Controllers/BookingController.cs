using Microsoft.AspNetCore.Mvc;
using P4.BookingService.DTOs;
using P4.BookingService.Services.IServices;

namespace P4.BookingService.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost("book-seats")]
        public async Task<IActionResult> BookSeats([FromBody] BookingCreateRequestDTO request)
        {
            try
            {
                await _bookingService.Create(request);
                return Ok(new { message = "Booking created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
