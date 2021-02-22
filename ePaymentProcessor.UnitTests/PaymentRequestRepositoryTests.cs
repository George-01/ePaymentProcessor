using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using PaymentProcessorAPI.Models;
using PaymentProcessorAPI.Models.Dtos;
using PaymentProcessorAPI.Repository;
using PaymentProcessorAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ePaymentProcessor.UnitTests
{
    public class PaymentRequestRepositoryTests
    {
        IPaymentRequestRepository _paymentRequestRepository;

        Mock<IPaymentStateRepository> _paymentStateRepository;
        Mock<ICheapPaymentGateway> _cheapPaymentGateway;
        Mock<IExpensivePaymentGateway> _expensivePaymentGateway;
        Mock<IPaymentRepository> _paymentRepository;
        Mock<IMapper> _mapper;
        // Mock<ILogger<PaymentRequestRepositoryTests>> _logger;

        [SetUp]
        public void Setup()
        {
            _paymentStateRepository = new Mock<IPaymentStateRepository>();
            _paymentRepository = new Mock<IPaymentRepository>();
            _cheapPaymentGateway = new Mock<ICheapPaymentGateway>();
            _expensivePaymentGateway = new Mock<IExpensivePaymentGateway>();
            _mapper = new Mock<IMapper>();
            //_logger = new Mock<ILogger<PaymentRequestRepositoryTests>>();

            _paymentRequestRepository = new PaymentRequestRepository(_paymentRepository.Object, _paymentStateRepository.Object, _cheapPaymentGateway.Object, _expensivePaymentGateway.Object, _mapper.Object);

            _mapper.Setup(s => s.Map<PaymentCreateDto, Payment>(It.IsAny<PaymentCreateDto>())).Returns((PaymentCreateDto pc) => new Payment() { Amount = pc.Amount, CardHolder = pc.CardHolder, CreditCardNumber = pc.CreditCardNumber, ExpirationDate = pc.ExpirationDate, SecurityCode = pc.SecurityCode });
            _paymentRepository.Setup(v => v.CreatePaymentRecord(It.IsAny<Payment>())).Returns((Payment paymentEntity) => Task.FromResult(paymentEntity));
            _paymentStateRepository.Setup(m => m.CreatePaymentRecord(It.IsAny<PaymentState>())).Returns((PaymentState paymentStateEntity) => Task.FromResult(paymentStateEntity));
        }

        [Test, TestCaseSource(typeof(PaymentRequestRepositoryTestCaseSource), nameof(PaymentRequestRepositoryTestCaseSource.Tests))]
        public async Task Test_PaymentRequestRepository_ProcessPayment(PaymentCreateDto paymentRequestDto, PaymentStateDto cheapGatewayResponseDto, int timesCheapGatewayCalled, PaymentStateDto expensiveGatewayResponseDto, int timesExpensiveGatewayCalled, PaymentStateEnum expectedPaymentStateEnum)
        {
            //arrange

            _cheapPaymentGateway.Setup(s => s.ProcessPayment(paymentRequestDto)).Returns(cheapGatewayResponseDto);
            _expensivePaymentGateway.Setup(s => s.ProcessPayment(paymentRequestDto)).Returns(expensiveGatewayResponseDto);

            //act
            var paymentStateDto = await _paymentRequestRepository.MakePayment(paymentRequestDto);
            
            //assert
            Assert.IsNotNull(paymentStateDto);
            Assert.AreEqual(paymentStateDto.PaymentState, expectedPaymentStateEnum);
            _cheapPaymentGateway.Verify(s => s.ProcessPayment(paymentRequestDto), Times.Exactly(timesCheapGatewayCalled));
            _expensivePaymentGateway.Verify(s => s.ProcessPayment(paymentRequestDto), Times.Exactly(timesExpensiveGatewayCalled));
        }
    }

    public static class PaymentRequestRepositoryTestCaseSource
    {
        public static PaymentStateDto FailedPaymentStateDto { get { return new PaymentStateDto() { PaymentState = PaymentStateEnum.Failed, PaymentStateDate = DateTime.Now }; } }
        public static PaymentStateDto ProcessedPaymentStateDto { get { return new PaymentStateDto() { PaymentState = PaymentStateEnum.Processed, PaymentStateDate = DateTime.Now }; } }

        public static PaymentCreateDto FirstTierPaymentCreateDto { get { return new PaymentCreateDto() { Amount = 19, CardHolder = "Tracy Willer", CreditCardNumber = "2221001657019409", ExpirationDate = DateTime.Now.AddYears(2), SecurityCode = "328" }; } }
        public static PaymentCreateDto SecondTierPaymentCreateDto { get { return new PaymentCreateDto() { Amount = 21, CardHolder = "Tracy Willer", CreditCardNumber = "2221001657019409", ExpirationDate = DateTime.Now.AddYears(2), SecurityCode = "328" }; } }
        public static PaymentCreateDto LastTierPaymentCreateDto { get { return new PaymentCreateDto() { Amount = 501, CardHolder = "Tracy Willer", CreditCardNumber = "2221001657019409", ExpirationDate = DateTime.Now.AddYears(2), SecurityCode = "328" }; } }

        public static IEnumerable<TestCaseData> Tests
        {
            get
            {
                yield return new TestCaseData(FirstTierPaymentCreateDto, ProcessedPaymentStateDto, 1, ProcessedPaymentStateDto, 0, PaymentStateEnum.Processed).SetName("FirstTier_CheapProcessed_ExpensiveProcessed");
                yield return new TestCaseData(FirstTierPaymentCreateDto, FailedPaymentStateDto, 1, ProcessedPaymentStateDto, 0, PaymentStateEnum.Failed).SetName("FirstTier_CheapFailed_ExpensiveProcessed");
                yield return new TestCaseData(FirstTierPaymentCreateDto, FailedPaymentStateDto, 1, FailedPaymentStateDto, 0, PaymentStateEnum.Failed).SetName("FirstTier_CheapFailed_ExpensiveFailed");

                yield return new TestCaseData(SecondTierPaymentCreateDto, ProcessedPaymentStateDto, 0, ProcessedPaymentStateDto, 1, PaymentStateEnum.Processed).SetName("SecondTier_CheapProcessed_ExpensiveProcessed");
                yield return new TestCaseData(SecondTierPaymentCreateDto, FailedPaymentStateDto, 0, ProcessedPaymentStateDto, 1, PaymentStateEnum.Processed).SetName("SecondTier_CheapFailed_ExpensiveProcessed");
                yield return new TestCaseData(SecondTierPaymentCreateDto, FailedPaymentStateDto, 1, FailedPaymentStateDto, 1, PaymentStateEnum.Failed).SetName("SecondTier_CheapFailed_ExpensiveFailed");

                yield return new TestCaseData(LastTierPaymentCreateDto, ProcessedPaymentStateDto, 0, ProcessedPaymentStateDto, 1, PaymentStateEnum.Processed).SetName("LastTier_CheapProcessed_ExpensiveProcessed");
                yield return new TestCaseData(LastTierPaymentCreateDto, FailedPaymentStateDto, 0, ProcessedPaymentStateDto, 1, PaymentStateEnum.Processed).SetName("LastTier_CheapFailed_ExpensiveProcessed");
                yield return new TestCaseData(LastTierPaymentCreateDto, FailedPaymentStateDto, 0, FailedPaymentStateDto, 3, PaymentStateEnum.Failed).SetName("LastTier_CheapFailed_ExpensiveFailed");
            }
        }

    }
}
