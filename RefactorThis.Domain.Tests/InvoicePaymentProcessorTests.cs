using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using RefactorThis.API.Domain.Entities;
using RefactorThis.API.Domain.Repositories;
using RefactorThis.API.Domain.Services;

namespace RefactorThis.Domain.Tests
{
    [TestFixture]
    public class InvoicePaymentProcessorTests
    {
        private Mock<IInvoiceRepository> _mockRepo;
        private InvoiceService _invoiceService;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IInvoiceRepository>(); // A mock object of IInvoiceRepository, which simulates the repository's behavior.
            _invoiceService = new InvoiceService(_mockRepo.Object); //An instance of InvoiceService that uses the mocked repository.
        }

        [Test]
        public void ProcessPayment_Should_ThrowException_When_NoInvoiceFoundForPaymentReference()
        {
            _mockRepo.Setup(repo => repo.GetInvoice(It.IsAny<string>())).Returns((Invoice)null);

            var payment = new Payment { Reference = "INV123" };
            var ex = Assert.Throws<InvalidOperationException>(() => _invoiceService.ProcessPayment(payment)); // I just used the exception message from the ProcessPayment
            //Assert.Equal("There is no invoice matching this payment", failureMessage); // this is not the correct syntax or referencing in the current testing framework version that I installed in Nuget
            Assert.That(ex.Message, Is.EqualTo("There is no invoice matching this payment"));
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_NoPaymentNeeded()
        {
            var invoice = new Invoice { Reference = "INV123", Amount = 0, AmountPaid = 0, Payments = new List<Payment>() };
            _mockRepo.Setup(repo => repo.GetInvoice(It.IsAny<string>())).Returns(invoice);

            var payment = new Payment { Reference = "INV123" };
            var result = _invoiceService.ProcessPayment(payment);

            Assert.That(result, Is.EqualTo("no payment needed"));
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaid()
        {
            var invoice = new Invoice { Reference = "INV123", Amount = 10, AmountPaid = 10, Payments = new List<Payment> { new Payment { Amount = 10 } } };
            _mockRepo.Setup(repo => repo.GetInvoice(It.IsAny<string>())).Returns(invoice);

            var payment = new Payment { Reference = "INV123" };
            var result = _invoiceService.ProcessPayment(payment);

            Assert.That(result, Is.EqualTo("invoice was already fully paid"));
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_PartialPaymentExistsAndAmountPaidExceedsAmountDue()
        {
            var invoice = new Invoice { Reference = "INV123", Amount = 10, AmountPaid = 5, Payments = new List<Payment> { new Payment { Amount = 5 } } };
            _mockRepo.Setup(repo => repo.GetInvoice(It.IsAny<string>())).Returns(invoice);

            var payment = new Payment { Amount = 6, Reference = "INV123" };
            var result = _invoiceService.ProcessPayment(payment);

            Assert.That(result, Is.EqualTo("the payment is greater than the partial amount remaining"));
        }

        [Test]
        public void ProcessPayment_Should_ReturnFailureMessage_When_NoPartialPaymentExistsAndAmountPaidExceedsInvoiceAmount()
        {
            var invoice = new Invoice { Reference = "INV123", Amount = 5, AmountPaid = 0, Payments = new List<Payment>() };
            _mockRepo.Setup(repo => repo.GetInvoice(It.IsAny<string>())).Returns(invoice);

            var payment = new Payment { Amount = 6, Reference = "INV123" };
            var result = _invoiceService.ProcessPayment(payment);

            Assert.That(result, Is.EqualTo("the payment is greater than the invoice amount"));
        }

        [Test]
        public void ProcessPayment_Should_ReturnFullyPaidMessage_When_PartialPaymentExistsAndAmountPaidEqualsAmountDue()
        {
            var invoice = new Invoice { Reference = "INV123", Amount = 10, AmountPaid = 5, Payments = new List<Payment> { new Payment { Amount = 5 } } };
            _mockRepo.Setup(repo => repo.GetInvoice(It.IsAny<string>())).Returns(invoice);

            var payment = new Payment { Amount = 5, Reference = "INV123" };
            var result = _invoiceService.ProcessPayment(payment);

            Assert.That(result, Is.EqualTo("final partial payment received, invoice is now fully paid"));
        }

        [Test]
        public void ProcessPayment_Should_ReturnFullyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidEqualsInvoiceAmount()
        {
            var invoice = new Invoice { Reference = "INV123", Amount = 10, AmountPaid = 0, Payments = new List<Payment>() };
            _mockRepo.Setup(repo => repo.GetInvoice(It.IsAny<string>())).Returns(invoice);

            var payment = new Payment { Amount = 10, Reference = "INV123" };
            var result = _invoiceService.ProcessPayment(payment);

            Assert.That(result, Is.EqualTo("invoice is now fully paid"));
        }

        [Test]
        public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue()
        {
            var invoice = new Invoice { Amount = 10, AmountPaid = 5, Payments = new List<Payment> { new Payment { Amount = 5 } } };
            _mockRepo.Setup(repo => repo.GetInvoice(It.IsAny<string>())).Returns(invoice);

            var payment = new Payment { Amount = 1, Reference = "INV123" };
            var result = _invoiceService.ProcessPayment(payment);

            Assert.That(result, Is.EqualTo("another partial payment received, still not fully paid"));
        }

        [Test]
        public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount()
        {
            var invoice = new Invoice { Reference = "INV123", Amount = 10, AmountPaid = 0, Payments = new List<Payment>() };
            _mockRepo.Setup(repo => repo.GetInvoice(It.IsAny<string>())).Returns(invoice);

            var payment = new Payment { Amount = 1, Reference = "INV123" };
            var result = _invoiceService.ProcessPayment(payment);

            Assert.That(result, Is.EqualTo("invoice is now partially paid"));
        }
    }
}
