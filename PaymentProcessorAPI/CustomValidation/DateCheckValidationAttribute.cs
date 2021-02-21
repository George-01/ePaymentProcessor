using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentProcessorAPI.CustomValidation
{
    public class DateCheckValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object dateValue)
        {
            DateTime d = Convert.ToDateTime(dateValue);
            return d >= DateTime.Now;

        }
    }
}
