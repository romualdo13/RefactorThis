using System;
using RefactorThis.Domain.Entities;
using RefactorThis.Domain.Repositories;

namespace RefactorThis.Domain.Services
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
            var invoice = _invoiceRepository.GetInvoice(payment.Reference);

            if (invoice == null)
                throw new InvalidOperationException("There is no invoice matching this payment");

            var result = invoice.ProcessPayment(payment);
            _invoiceRepository.SaveInvoice(invoice);
            return result;
        }
    }
}
