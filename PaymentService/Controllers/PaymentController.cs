using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService.DTOs;
using PaymentService.Services.IServices;
using Stripe;

namespace PaymentService.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService, IConfiguration configuration)
        {
            _paymentService = paymentService;
            _configuration = configuration;
        }

        [HttpPost("create-payment-intent-id")]
        public async Task<IActionResult> CreatePaymentItentId([FromBody] PaymentRequestDTO paymentRequestDTO)
        {
            try
            {
                var paymentIntent = await _paymentService.CreatePaymentIntentAsync(paymentRequestDTO);
                return Ok(new { paymentItentId = paymentIntent.Id });
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }   
        }

        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var whSecret = _configuration["Stripe:WhSecret"];
            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], whSecret);
            try
            {
                await _paymentService.HandleStripeWebhookAsync(stripeEvent);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
