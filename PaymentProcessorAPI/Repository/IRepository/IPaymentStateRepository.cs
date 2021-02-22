using PaymentProcessorAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentProcessorAPI.Repository.IRepository
{
    public interface IPaymentStateRepository
    {
        ICollection<PaymentState> GetAllPaymentData();

        PaymentState GetPaymentDataById(int paymentStateId);

        Task<PaymentState> CreatePaymentRecord(PaymentState paymentState);

        bool UpdatePaymentRecord(PaymentState paymentState);

        bool DeletePaymentRecord(PaymentState paymentStatement);

        bool Save();
    }
}
