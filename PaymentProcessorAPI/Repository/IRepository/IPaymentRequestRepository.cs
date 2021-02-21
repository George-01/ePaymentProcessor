using PaymentProcessorAPI.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentProcessorAPI.Repository.IRepository
{
    public interface IPaymentRequestRepository
    {
        Task<PaymentStateDto> MakePayment(PaymentCreateDto paymentCreateDto);
    }
}
