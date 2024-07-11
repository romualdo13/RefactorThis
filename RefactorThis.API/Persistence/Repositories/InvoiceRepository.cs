using RefactorThis.API.Domain.Entities;
using RefactorThis.API.Domain.Repositories;

namespace RefactorThis.API.Persistence.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        // Can use dbContext here but for this we will not use dbContext as we only test the logic. something like this 

        //private readonly DbContext _context;

        //public InvoiceRepository(DbContext context)
        //{
        //    _context = context;
        //}

        private Invoice _invoice;

        public InvoiceRepository()
        {
            // Initialize _invoice with some data
            _invoice = new Invoice
            {
                Reference = "INV123",
                Amount = 10
            };
        }
        public Invoice GetInvoice(string reference)
        {
            if (reference == _invoice.Reference)
            {
                return _invoice;
            }
            else
            {
                return null;
            }
        }

        public void SaveInvoice(Invoice invoice)
        {
            // Saves the invoice to the database
        }

        public void Add(Invoice invoice)
        {
            _invoice = invoice;
        }
    }
}
