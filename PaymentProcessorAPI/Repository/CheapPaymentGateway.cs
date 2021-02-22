using PaymentProcessorAPI.Models;
using PaymentProcessorAPI.Models.Dtos;
using PaymentProcessorAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentProcessorAPI.Repository
{
    public class CheapPaymentGateway : ICheapPaymentGateway
    {
        public PaymentStateDto ProcessPayment(PaymentCreateDto paymentRequest)
        {
            Random rnd = new Random();
            var num = rnd.Next(1, 12);
            if (num % 4 == 0 || num % 6 == 0)
                throw new Exception("Call failed");
            if (num % 2 == 0)
                return new PaymentStateDto() { PaymentState = PaymentStateEnum.Failed, PaymentStateDate = DateTime.Now };
            else return new PaymentStateDto() { PaymentState = PaymentStateEnum.Processed, PaymentStateDate = DateTime.Now };
        }
    }
}
