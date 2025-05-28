using Contracts.BookingEvents;
using MassTransit;
using P4.BookingService.DTOs;
using P4.BookingService.Services.IServices;

namespace P4.BookingService.Consumers
{
    public class BookingCreateCommandConsumer : IConsumer<BookingCreateCommand>, IConsumer<Fault<BookingCreateCommand>>
    {
        private readonly ILogger<BookingCreateCommandConsumer> _logger;
        private readonly IBookingService _bookingService;
        private readonly IPublishEndpoint _publishEndpoint;

        public BookingCreateCommandConsumer(ILogger<BookingCreateCommandConsumer> logger, IBookingService bookingService, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _bookingService = bookingService;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<BookingCreateCommand> context)
        {
            try
            {
                _logger.LogInformation($"---> Consume booking create command for user {context.Message.UserId}");
                var request = new BookingCreateRequestDTO
                {
                    BookingId = context.Message.BookingId,
                    UserId = context.Message.UserId,
                    ScreeningId = context.Message.ScreeningId,
                    Seats = context.Message.SeatIds,
                    PaymentIntentId = context.Message.PaymentIntentId
                };
                await _bookingService.Create(request);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task Consume(ConsumeContext<Fault<BookingCreateCommand>> context)
        {
            var message = context.Message.Message;
            _logger.LogError("BookingCreateCommand failed after retries for PaymentIntent {PaymentIntentId}", message.PaymentIntentId);

            await _publishEndpoint.Publish(new FailedSagaEvent(message.BookingId));
        }
    }
}
