using Contracts.BookingEvents;
using MassTransit;

namespace SearchService.Consumers
{
    public class BookingFailedConsumer : IConsumer<BookingFailed>
    {
        private readonly ILogger<BookingFailedConsumer> _logger;

        public BookingFailedConsumer(ILogger<BookingFailedConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<BookingFailed> context)
        {
            _logger.LogInformation($"---> Consuming success for Booking Failed Message");   
        }
    }
}
