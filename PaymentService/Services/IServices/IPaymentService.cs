using PaymentService.DTOs;
using Stripe;

namespace PaymentService.Services.IServices
{
    public interface IPaymentService
    {
        Task<PaymentIntent> CreatePaymentIntentAsync(PaymentRequestDTO paymentRequestDTO);
        Task HandleStripeWebhookAsync(Event stripeEvent);
    }
}
