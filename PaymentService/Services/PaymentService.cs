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

        public PaymentService(IConnectionMultiplexer redis, IPublishEndpoint publishEndpoint)
        {
            _redis = redis.GetDatabase();
            _publishEndpoint = publishEndpoint;
        }
        public async Task<PaymentIntent> CreatePaymentIntentAsync(PaymentRequestDTO request)
        {
            #region Race Condition

            foreach (var seat in request.Seats)
            {
                var key = $"seat:{seat}:screen:{request.ScreeningId}";
                var existingValue = await _redis.StringGetAsync(key);
                if (existingValue != RedisValue.Null && existingValue != request.UserId.ToString())
                    throw new ArgumentException($"Seat {seat} is not available");

                var held = await _redis.StringSetAsync(key, request.UserId.ToString(), TimeSpan.FromSeconds(100), When.NotExists);
                if (!held)
                    throw new ArgumentException($"Seat {seat} is not held successfully ! someone is held");
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
                var paymentRequestDTO = JsonConvert.DeserializeObject<PaymentRequestDTO>(paymentIntent.Metadata["PaymentRequestDTO"]);

                await _publishEndpoint.Publish(new BookingCreateCommand(paymentRequestDTO.UserId, paymentRequestDTO.ScreeningId, paymentRequestDTO.Seats));
            }
            else if (stripeEvent.Type == "payment_intent.payment_failed")
            {

            }
        }
    }
}
