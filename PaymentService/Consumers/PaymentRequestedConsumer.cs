using Contracts.BookingEvents;
using MassTransit;
using PaymentService.Services.IServices;

namespace PaymentService.Consumers
{
    public class PaymentRequestedConsumer : IConsumer<PaymentRequested>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<PaymentRequestedConsumer> _logger;
        private readonly IPaymentService _paymentService;

        public PaymentRequestedConsumer(IPublishEndpoint publishEndpoint, ILogger<PaymentRequestedConsumer> logger, IPaymentService paymentService)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
            _paymentService = paymentService;
        }

        public async Task Consume(ConsumeContext<PaymentRequested> context)
        {
            _logger.LogInformation($"---> Consume Payment requested {context.Message.BookingId}");

            bool isSuccess = true;
            // Will update stripe payment gateway later


            if (isSuccess)
            {
                _logger.LogInformation($"---> Payment success for booking: {context.Message.BookingId}");
                await _publishEndpoint.Publish(new PaymentCompleted(context.Message.BookingId));
            }
            else
            {
                _logger.LogInformation($"---> Payment failed for booking: {context.Message.BookingId}");
                await _publishEndpoint.Publish(new PaymentFailed(context.Message.BookingId));
            }
        }
    }
}
