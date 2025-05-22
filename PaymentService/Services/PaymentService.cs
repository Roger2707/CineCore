using PaymentService.Services.IServices;
using Stripe;

namespace PaymentService.Services
{
    public class PaymentService : IPaymentService
    {
        public Task<PaymentIntent> CreatePaymentIntentAsync(decimal amount)
        {
            throw new NotImplementedException();
        }

        public Task HandleStripeWebhookAsync(Event stripeEvent)
        {
            throw new NotImplementedException();
        }
    }
}
