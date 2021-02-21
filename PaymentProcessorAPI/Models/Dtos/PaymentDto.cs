using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentProcessorAPI.Models.Dtos
{
    public class PaymentDto
    {
        public bool IsProcessed { get; set; }
        public PaymentStateDto PaymentState { get; set; }
    }
}
