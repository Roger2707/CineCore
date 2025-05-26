using CinemaService.Services.IServices;
using Contracts.BookingEvents;
using MassTransit;

namespace CinemaService.Consumers
{
    public class UpdateSeatStatusConsumer : IConsumer<UpdateSeatStatus>, IConsumer<Fault<UpdateSeatStatus>>
    {
        private readonly ILogger<UpdateSeatStatusConsumer> _logger;
        private readonly IScreeingService _screeingService;
        private readonly IPublishEndpoint _publishEndpoint;

        public UpdateSeatStatusConsumer(ILogger<UpdateSeatStatusConsumer> logger, IScreeingService screeingService, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _screeingService = screeingService;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<UpdateSeatStatus> context)
        {
            _logger.LogInformation("Received UpdateSeatStatus event !!!");
            try
            {
                await _screeingService.UpdateScreeningSeatStatus(context.Message);
                await _publishEndpoint.Publish(new SeatUpdateCompleted(context.Message.BookingId, context.Message.ScreeningId, context.Message.Seats, context.Message.PaymentIntentId));
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error while consuming UpdateSeatStatus event");
                throw;
            }
        }

        public async Task Consume(ConsumeContext<Fault<UpdateSeatStatus>> context)
        {
            var message = context.Message.Message;
            _logger.LogInformation("Received UpdateSeatStatus Failed event !!!");
            await _publishEndpoint.Publish(new SeatUpdateFailed(message.BookingId, message.ScreeningId, message.Seats, message.PaymentIntentId));
        }
    }
}
