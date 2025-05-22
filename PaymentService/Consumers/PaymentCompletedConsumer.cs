using Contracts.BookingEvents;
using MassTransit;

namespace PaymentService.Consumers
{
    public class PaymentCompletedConsumer : IConsumer<PaymentCompleted>
    {
        private readonly ILogger<PaymentCompletedConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public PaymentCompletedConsumer(ILogger<PaymentCompletedConsumer> logger, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<PaymentCompleted> context)
        {
            _logger.LogInformation($"---> Consume payment success event for booking {context.Message.BookingId}");
            await _publishEndpoint.Publish(new BookingFinished(context.Message.BookingId, BookingStatus.SUCCESSED));
        }
    }
}
