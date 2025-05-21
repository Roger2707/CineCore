using Contracts.BookingEvents;
using MassTransit;

namespace OrchestrationSaga.TestConsumers
{
    public class BookingCreatedConsumer : IConsumer<BookingCreated>
    {
        public async Task Consume(ConsumeContext<BookingCreated> context)
        {
            Console.WriteLine($"---> TEST booking failed: bookingId : {context.Message.BookingId}");
        }
    }
}
