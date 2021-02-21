using PaymentProcessorAPI.Models.Dtos;
using PaymentProcessorAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentProcessorAPI.Repository
{
    public class PaymentRequestRepository : IPaymentRequestRepository
    {

        public PaymentRequestRepository()
        {

        }
        public Task<PaymentStateDto> MakePayment(PaymentCreateDto paymentCreateDto)
        {
            throw new NotImplementedException();
        }
    }
}
