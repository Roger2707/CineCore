using Contracts.BookingEvents;
using MassTransit;
using Newtonsoft.Json;
using PaymentService.DTOs;
using PaymentService.Services.IServices;
using StackExchange.Redis;
using Stripe;

namespace PaymentService.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IDatabase _redis;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(IConnectionMultiplexer redis, IPublishEndpoint publishEndpoint, ILogger<PaymentService> logger)
        {
            _redis = redis.GetDatabase();
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }
        public async Task<PaymentIntent> CreatePaymentIntentAsync(PaymentRequestDTO request)
        {
            #region Validate and Extend Seat Hold

            foreach (var seat in request.Seats)
            {
                var key = $"seat:{seat}:screen:{request.ScreeningId}";
                var existingValue = await _redis.StringGetAsync(key);

                // Seat must be held by the same user
                if (existingValue != request.UserId.ToString())
                    throw new ArgumentException($"Seat {seat} is not held by you or has expired");

                // Extend TTL for payment processing (10 minutes)
                await _redis.StringSetAsync(key, request.UserId.ToString(), TimeSpan.FromMinutes(10));
            }

            #endregion

            #region Exchange Amount (VND -> USD)

            decimal totalPrice = request.Seats.Count * 139000000;
            decimal exchangeRate = 0.000039m;
            long amountInCents = (long)(totalPrice * exchangeRate * 100);

            #endregion

            #region Create Payment Intent

            var options = new PaymentIntentCreateOptions
            {
                Amount = amountInCents,
                Currency = "usd",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions{ Enabled = true },
                Metadata = new Dictionary<string, string>
                {
                    { "PaymentRequestDTO", JsonConvert.SerializeObject(request) },
                    { "RequestId", Guid.NewGuid().ToString() } // For idempotency
                }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            #endregion

            return paymentIntent;
        }

        public async Task HandleStripeWebhookAsync(Stripe.Event stripeEvent)
        {
            if (stripeEvent.Type == "payment_intent.succeeded")
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                if (paymentIntent == null) return;

                // Check idempotency
                var requestId = paymentIntent.Metadata.GetValueOrDefault("RequestId");
                var processed = await _redis.StringGetAsync($"processed:{paymentIntent.Id}");
                if (processed.HasValue)
                {
                    _logger.LogInformation("Payment already processed: {PaymentIntentId}", paymentIntent.Id);
                    return;
                }

                // Mark as processing to prevent duplicate
                await _redis.StringSetAsync($"processed:{paymentIntent.Id}", "true", TimeSpan.FromHours(24));

                var paymentRequestDTO = JsonConvert.DeserializeObject<PaymentRequestDTO>(
                    paymentIntent.Metadata["PaymentRequestDTO"]);

                await _publishEndpoint.Publish(new BookingCreateCommand(
                    Guid.NewGuid(),
                    paymentRequestDTO.UserId,
                    paymentRequestDTO.ScreeningId,
                    paymentRequestDTO.Seats,
                    paymentIntent.Id 
                ));
            }
            else if (stripeEvent.Type == "payment_intent.payment_failed")
            {

            }
        }
    }
}
