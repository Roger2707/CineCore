using BookingService.Services.IServices;
using Contracts.BookingEvents;
using MassTransit;

namespace BookingService.Consumers
{
    public class SeatHoldRequestedConsumer : IConsumer<SeatHoldRequested>
    {
        private readonly IBookingService _bookingService;
        public SeatHoldRequestedConsumer(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }
        public async Task Consume(ConsumeContext<SeatHoldRequested> context)
        {
            Console.WriteLine("---> Consume receive seat hold requested event");

            bool success = Random.Shared.Next(2) == 0;
            if (success)
                await context.Publish(new SeatHoldCompleted(context.Message.BookingId));
            else
                await context.Publish(new SeatHoldFailed(context.Message.BookingId));
        }
    }
}
