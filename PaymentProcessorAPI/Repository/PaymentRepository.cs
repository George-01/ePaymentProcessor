using PaymentProcessorAPI.Data;
using PaymentProcessorAPI.Models;
using PaymentProcessorAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentProcessorAPI.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _db;

        public PaymentRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<Payment> CreatePaymentRecord(Payment payment)
        {
            var objData = await _db.Set<Payment>().AddAsync(payment);
            await _db.SaveChangesAsync();
            return objData.Entity;
        }

        public bool DeletePaymentRecord(Payment payment)
        {
            _db.Payments.Remove(payment);
            return Save();
        }

        public ICollection<Payment> GetAllPaymentData()
        {
            return _db.Set<Payment>().OrderBy(a => a.CreditCardNumber).ToList();
        }

        public Payment GetPaymentDataById(int paymentId)
        {
            return _db.Set<Payment>().FirstOrDefault(a => a.PaymentId == paymentId);
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }

        public bool UpdatePaymentRecord(Payment paymment)
        {
            _db.Payments.Update(paymment);
            return Save();
        }
    }
}
