using P4.BookingService.Services.IServices;
using Contracts.BookingEvents;
using MassTransit;

namespace P4.BookingService.Consumers
{
    public class ProcessingFailedSagaConsumer : IConsumer<ProcessingFailedSaga>, IConsumer<Fault<ProcessingFailedSaga>>
    {
        private readonly ILogger<ProcessingFailedSagaConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IBookingService _bookingService;

        public ProcessingFailedSagaConsumer(ILogger<ProcessingFailedSagaConsumer> logger, IPublishEndpoint publishEndpoint, IBookingService bookingService)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
            _bookingService = bookingService;
        }

        public async Task Consume(ConsumeContext<ProcessingFailedSaga> context)
        {
            _logger.LogInformation("Received ProcessingFailedSaga event !!!");
            
            try
            {
                await _bookingService.Delete(context.Message.BookingId, context.Message.Seats, context.Message.ScreeningId);
                await _publishEndpoint.Publish(new FailedSagaEvent(context.Message.BookingId));
                await _bookingService.SaveChangeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing ProcessingFailedSaga event");
                throw;
            }
        }

        public Task Consume(ConsumeContext<Fault<ProcessingFailedSaga>> context)
        {
            throw new NotImplementedException();
        }
    }
}
