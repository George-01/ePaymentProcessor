using PaymentProcessorAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentProcessorAPI.Repository.IRepository
{
    public interface IPaymentRepository
    {
        ICollection<Payment> GetAllPaymentData();

        Payment GetPaymentDataById(int paymentId);

        Task<Payment> CreatePaymentRecord(Payment payment);

        bool UpdatePaymentRecord(Payment paymment);

        bool DeletePaymentRecord(Payment paymment);

        bool Save();

        /*
         ICollection<PaymentData> GetAllPaymentData();

        PaymentData GetPaymentData(int paymentDataId);

        bool PaymentDataExists(string name);

        bool PaymentDataExists(int paymentId);

        bool CreatePaymentRecord(PaymentData paymentData);

        bool UpdatePaymentRecord(PaymentData paymentData);

        bool DeletePaymentRecord(PaymentData paymentData);

        bool Save();     
         */
    }
}
