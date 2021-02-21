using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentProcessorAPI.Models.Dtos
{
    public class PaymentStateDto
    {
        public PaymentStateEnum PaymentState { get; set; }
        public DateTime PaymentStateDateCreated { get; set; }
    }
}
