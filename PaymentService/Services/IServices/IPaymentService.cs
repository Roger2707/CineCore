using P5.PaymentService.DTOs;
using Stripe;

namespace P5.PaymentService.Services.IServices
{
    public interface IPaymentService
    {
        Task<PaymentIntent> CreatePaymentIntentAsync(PaymentRequestDTO paymentRequestDTO);
        Task HandleStripeWebhookAsync(Event stripeEvent);
    }
}
