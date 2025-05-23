using Contracts.BookingEvents;
using EmailSendingService.Services;
using MassTransit;

namespace EmailSendingService.Consumers
{
    public class EmailTicketCreatedConsumer : IConsumer<EmailTicketCreated>
    {
        private readonly ILogger<EmailTicketCreatedConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly EmailService _emailService;

        public EmailTicketCreatedConsumer(ILogger<EmailTicketCreatedConsumer> logger, IPublishEndpoint publishEndpoint, EmailService emailService)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
            _emailService = emailService;
        }

        public async Task Consume(ConsumeContext<EmailTicketCreated> context)
        {
            _logger.LogInformation("EmailTicketCreatedConsumer: {BookingId} - {UserEmail}", context.Message.BookingId, context.Message.UserEmail);
            await _emailService.SendTicketAsync(context.Message);
            await _publishEndpoint.Publish(new TicketDelivered(context.Message.BookingId));
        }
    }
}
