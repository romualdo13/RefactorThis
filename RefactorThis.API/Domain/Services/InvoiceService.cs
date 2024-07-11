using System;
using RefactorThis.API.Domain.Entities;
using RefactorThis.API.Domain.Repositories;

namespace RefactorThis.API.Domain.Services
{
    public class InvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoiceService(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public string ProcessPayment(Payment payment)
        {
            var invoice = _invoiceRepository.GetInvoice(payment.Reference); // Check the reference

            if (invoice == null) // Checking if the invoice is null
                throw new InvalidOperationException("There is no invoice matching this payment");
            // If not null proceed to ProcessPayment
            var responseMessage = invoice.ProcessPayment(payment);
            _invoiceRepository.SaveInvoice(invoice);
            return responseMessage;
        }
    }
}
