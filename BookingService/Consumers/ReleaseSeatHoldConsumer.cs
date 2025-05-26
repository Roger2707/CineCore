using BookingService.DTOs;
using BookingService.Services.IServices;
using Contracts.BookingEvents;
using MassTransit;

namespace BookingService.Consumers
{
    public class ReleaseSeatHoldConsumer : IConsumer<ReleaseSeatHold>
    {
        private readonly ILogger<ReleaseSeatHoldConsumer> _logger;
        private readonly IBookingService _bookingService;

        public ReleaseSeatHoldConsumer(ILogger<ReleaseSeatHoldConsumer> logger, IBookingService bookingService)
        {
            _logger = logger;
            _bookingService = bookingService;
        }

        public async Task Consume(ConsumeContext<ReleaseSeatHold> context)
        {
            _logger.LogInformation($"---> Consume booking create command for user {context.Message.ScreeningId}");

            // Handle Later
        }
    }
}
