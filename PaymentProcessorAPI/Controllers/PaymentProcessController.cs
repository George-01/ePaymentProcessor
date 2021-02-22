using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentProcessorAPI.Models;
using PaymentProcessorAPI.Models.Dtos;
using PaymentProcessorAPI.Repository.IRepository;

namespace PaymentProcessorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentProcessController : ControllerBase
    {
        private readonly ILogger<PaymentProcessController> _logger;
        private readonly IPaymentRequestRepository _paymentRequestRepo;

        public PaymentProcessController(ILogger<PaymentProcessController> logger, IPaymentRequestRepository paymentRequestRepo)
        {
            _logger = logger;
            _paymentRequestRepo = paymentRequestRepo;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(PaymentCreateDto paymentRequest)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var paymentState = await _paymentRequestRepo.MakePayment(paymentRequest);
                    var paymentResponse = new PaymentDto()
                    {
                        IsProcessed = paymentState.PaymentState == PaymentStateEnum.Processed,
                        PaymentState = paymentState
                    };

                    if (!paymentResponse.IsProcessed)
                        return StatusCode(500, new { error = "Payment could not be processed" });
                    return Ok(paymentResponse);
                }
                else
                    return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500);
            }
        }
    }
}