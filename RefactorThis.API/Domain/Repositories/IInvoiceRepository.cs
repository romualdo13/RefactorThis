using RefactorThis.API.Domain.Entities;

namespace RefactorThis.API.Domain.Repositories
{
    public interface IInvoiceRepository
    {
        Invoice GetInvoice(string reference);
        void SaveInvoice(Invoice invoice);
    }
}
