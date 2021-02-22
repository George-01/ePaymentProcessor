using AutoMapper;
using PaymentProcessorAPI.Models;
using PaymentProcessorAPI.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentProcessorAPI.PaymentProcessorMapper
{
    public class PaymentProcessorMappings : Profile
    {
        public PaymentProcessorMappings()
        {
            CreateMap<Payment, PaymentCreateDto>().ReverseMap();

            CreateMap<Payment, PaymentListDto>().ReverseMap();

            CreateMap<PaymentState, PaymentStateDto>().ReverseMap();
        }
    }
}
