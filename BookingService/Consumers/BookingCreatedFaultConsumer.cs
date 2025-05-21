using BookingService.Services.IServices;
using Contracts.BookingEvents;
using MassTransit;

namespace BookingService.Consumers
{
    public class BookingCreatedFaultConsumer : IConsumer<BookingFailed>
    {
        private readonly IBookingService _bookingService;

        public BookingCreatedFaultConsumer(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public async Task Consume(ConsumeContext<BookingFailed> context)
        {
            Console.WriteLine($"---> Consume booking failed: bookingId : {context.Message.BookingId}");
            try
            {
                await _bookingService.Delete(context.Message.BookingId, context.Message.SeatIds, context.Message.ScreeningId);
                Console.WriteLine($"Delete Booking : {context.Message.BookingId} successfully !");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"---> Error: {ex.Message}");
            }
        }
    }
}
