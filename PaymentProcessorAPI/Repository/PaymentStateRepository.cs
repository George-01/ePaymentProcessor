using PaymentProcessorAPI.Data;
using PaymentProcessorAPI.Models;
using PaymentProcessorAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentProcessorAPI.Repository
{
    public class PaymentStateRepository : IPaymentStateRepository
    {
        private readonly ApplicationDbContext _db;

        public PaymentStateRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<PaymentState> CreatePaymentRecord(PaymentState paymentState)
        {
            var objPymntState = await _db.Set<PaymentState>().AddAsync(paymentState);
            await _db.SaveChangesAsync();
            return objPymntState.Entity;
        }

        public bool DeletePaymentRecord(PaymentState paymentStatement)
        {
            _db.PaymentStates.Remove(paymentStatement);
            return Save();
        }

        public ICollection<PaymentState> GetAllPaymentData()
        {
            return _db.Set<PaymentState>().OrderBy(a => a.PaymentId).ToList();
        }

        public PaymentState GetPaymentDataById(int paymentStateId)
        {
            return _db.Set<PaymentState>().FirstOrDefault(a => a.PaymentId == paymentStateId);
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }

        public bool UpdatePaymentRecord(PaymentState paymentState)
        {
            _db.PaymentStates.Update(paymentState);
            return Save();
        }
    }
}
