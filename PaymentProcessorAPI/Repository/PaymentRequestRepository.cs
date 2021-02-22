using AutoMapper;
using Microsoft.Extensions.Logging;
using PaymentProcessorAPI.Models;
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
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentStateRepository _paymentStateRepository;
        private readonly ICheapPaymentGateway _cheapPaymentGateway;
        private readonly IExpensivePaymentGateway _expensivePaymentGateway;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentRequestRepository> _logger;

        public PaymentRequestRepository(IPaymentRepository paymentRepository,IPaymentStateRepository paymentStateRepository,
                                        ICheapPaymentGateway cheapPaymentGateway,IExpensivePaymentGateway expensivePaymentGateway,
                                        IMapper mapper, ILogger<PaymentRequestRepository> logger)
        {
            _paymentRepository = paymentRepository;
            _paymentStateRepository = paymentStateRepository;
            _cheapPaymentGateway = cheapPaymentGateway;
            _expensivePaymentGateway = expensivePaymentGateway;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<PaymentStateDto> MakePayment(PaymentCreateDto paymentCreateDto)
        {
            var paymentEntity = _mapper.Map<PaymentCreateDto, Payment>(paymentCreateDto);
            paymentEntity = await _paymentRepository.CreatePaymentRecord(paymentEntity);

            var paymentStateEntity = new PaymentState() { Payment = paymentEntity, PaymentId = paymentEntity.PaymentId, DateCreated = DateTime.Now, State = PaymentStateEnum.Pending.ToString() };
            paymentStateEntity = await _paymentStateRepository.CreatePaymentRecord(paymentStateEntity);

            //implement payment gateway processing and make records persistent on DB
            if (paymentCreateDto.Amount <= 20)
            {
                var paymentStateDto = await ProcessPaymentStateDto(_cheapPaymentGateway, paymentCreateDto, paymentEntity);
                return paymentStateDto;
            }
            else if (paymentCreateDto.Amount > 20 && paymentCreateDto.Amount <= 500)
            {
                PaymentStateDto paymentStateDto = new PaymentStateDto() { PaymentState = PaymentStateEnum.Failed, PaymentStateDate = DateTime.Now };
                int tryCount = 0;
                try
                {
                    paymentStateDto = await ProcessPaymentStateDto(_expensivePaymentGateway, paymentCreateDto, paymentEntity);
                    if (paymentStateDto != null && paymentStateDto.PaymentState == PaymentStateEnum.Processed)
                        return paymentStateDto;
                    else
                    {
                        tryCount++;
                        paymentStateDto = await ProcessPaymentStateDto(_cheapPaymentGateway, paymentCreateDto, paymentEntity);
                        return paymentStateDto;
                    }
                }
                catch (Exception ex)
                {
                    if (tryCount == 0)
                    {
                        paymentStateDto = await ProcessPaymentStateDto(_cheapPaymentGateway, paymentCreateDto, paymentEntity);
                        return paymentStateDto;
                    }
                }
                return paymentStateDto;
            }
            else
            {
                int tryCount = 0;
                PaymentStateDto paymentStateDto = new PaymentStateDto() { PaymentState = PaymentStateEnum.Failed, PaymentStateDate = DateTime.Now }; ;
                while (tryCount < 3)
                {
                    try
                    {
                        paymentStateDto = await ProcessPaymentStateDto(_expensivePaymentGateway, paymentCreateDto, paymentEntity);
                        if (paymentStateDto != null && paymentStateDto.PaymentState == PaymentStateEnum.Processed)
                            return paymentStateDto;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                    finally
                    {
                        tryCount++;
                    }
                }
                return paymentStateDto;
            }
            throw new Exception("Payment could not be processed");
        }

        private async Task<PaymentStateDto> ProcessPaymentStateDto(IPaymentGateway paymentGateway, PaymentCreateDto paymentCreateDto, Payment paymentEntity)
        {
            var paymentStateDto = paymentGateway.ProcessPayment(paymentCreateDto);
            var paymentStateEntityProcessed = new PaymentState() { Payment = paymentEntity, PaymentId = paymentEntity.PaymentId, DateCreated = paymentStateDto.PaymentStateDate, State = paymentStateDto.PaymentState.ToString() };
            paymentStateEntityProcessed = await _paymentStateRepository.CreatePaymentRecord(paymentStateEntityProcessed);
            return paymentStateDto;
        }
    }
}
