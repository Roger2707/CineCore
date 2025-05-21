using Contracts.BookingEvents;
using MassTransit;

namespace PaymentService.Consumers
{
    public class PaymentRequestedConsumer : IConsumer<PaymentRequested>
    {
        public Task Consume(ConsumeContext<PaymentRequested> context)
        {
            Console.WriteLine($"---> Consume Payment requested {context.Message.BookingId}");
            return Task.CompletedTask;
        }
    }
}
