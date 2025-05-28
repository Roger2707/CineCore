using Contracts.BookingEvents;
using MassTransit;

namespace P5.PaymentService.Consumers
{
    public class PaymentRefundConsumer : IConsumer<PaymentRefund>
    {
        private readonly ILogger<PaymentRefundConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public PaymentRefundConsumer(ILogger<PaymentRefundConsumer> logger, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<PaymentRefund> context)
        {
            _logger.LogInformation("Received PaymentRefund event for BookingId: {BookingId}", context.Message.BookingId);   
            
            // Will add stripe refund event later

            await _publishEndpoint.Publish(
                new FailedSagaEvent(context.Message.BookingId)
            );
        }
    }
}
