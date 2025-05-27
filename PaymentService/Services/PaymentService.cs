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
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;

        public PaymentService(IConnectionMultiplexer redis, IPublishEndpoint publishEndpoint, ILogger<PaymentService> logger, IConfiguration configuration)
        {
            _redis = redis.GetDatabase();
            _publishEndpoint = publishEndpoint;
            _logger = logger;
            _configuration = configuration;
            _apiKey = _configuration["Stripe:SecretKey"];

            if (string.IsNullOrEmpty(_apiKey)) throw new Exception("Stripe API Key is missing from configuration.");
            if (string.IsNullOrEmpty(StripeConfiguration.ApiKey))
                StripeConfiguration.ApiKey = _apiKey;
        }
        public async Task<PaymentIntent> CreatePaymentIntentAsync(PaymentRequestDTO request)
        {
            #region Validate and Extend Seat Hold

            foreach (var seat in request.Seats)
            {
                var key = $"seat:{seat}:screen:{request.ScreeningId}";
                var existingValue = await _redis.StringGetAsync(key);

                if(string.IsNullOrEmpty(existingValue) || existingValue == request.UserId.ToString())
                {
                    await _redis.StringSetAsync(key, request.UserId.ToString(), TimeSpan.FromMinutes(10));
                }
                else throw new ArgumentException($"Seat {seat} is not held by you or has expired");
            }

            #endregion

            #region Exchange Amount (VND -> USD)

            decimal totalPrice = request.Seats.Count * 139000;
            decimal exchangeRate = 0.000039m;
            long amountInCents = (long)(totalPrice * exchangeRate * 100);

            #endregion

            #region Create Payment Intent

            var options = new PaymentIntentCreateOptions
            {
                Amount = amountInCents,
                Currency = "usd",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions{ Enabled = true, AllowRedirects = "never" },
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
