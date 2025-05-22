using Contracts.BookingEvents;
using MassTransit;

namespace PaymentService.Consumers
{
    public class PaymentFailedConsumer : IConsumer<PaymentFailed>
    {
        private readonly ILogger<PaymentFailedConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public PaymentFailedConsumer(ILogger<PaymentFailedConsumer> logger, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<PaymentFailed> context)
        {
            _logger.LogInformation($"---> Consume payment failed event for booking {context.Message.BookingId}");
            await _publishEndpoint.Publish(new BookingFinished(context.Message.BookingId, BookingStatus.CANCELLED));
        }
    }
}
