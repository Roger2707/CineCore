using Stripe;

namespace PaymentService.Services.IServices
{
    public interface IPaymentService
    {
        Task<PaymentIntent> CreatePaymentIntentAsync(decimal amount);
        Task HandleStripeWebhookAsync(Event stripeEvent);
    }
}
