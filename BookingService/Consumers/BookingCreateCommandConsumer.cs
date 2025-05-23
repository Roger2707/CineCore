using BookingService.DTOs;
using BookingService.Services.IServices;
using Contracts.BookingEvents;
using MassTransit;

namespace BookingService.Consumers
{
    public class BookingCreateCommandConsumer : IConsumer<BookingCreateCommand>
    {
        private readonly ILogger<BookingCreateCommandConsumer> _logger;
        private readonly IBookingService _bookingService;

        public BookingCreateCommandConsumer(ILogger<BookingCreateCommandConsumer> logger, IBookingService bookingService)
        {
            _logger = logger;
            _bookingService = bookingService;
        }

        public async Task Consume(ConsumeContext<BookingCreateCommand> context)
        {
            _logger.LogInformation($"---> Consume booking create command for user {context.Message.UserId}");
            var request = new BookingCreateRequestDTO
            {
                UserId = context.Message.UserId,
                ScreeningId = context.Message.ScreeningId,
                Seats = context.Message.SeatIds
            };
            await _bookingService.Create(request);
        }
    }
}
