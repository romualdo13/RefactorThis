using RefactorThis.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RefactorThis.Domain.Entities
{
    public class Invoice
    {
        public decimal Amount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal TaxAmount { get; set; }
        public List<Payment> Payments { get; set; } = new List<Payment>();
        public InvoiceType Type { get; set; }

        public string ProcessPayment(Payment payment)
        {
            if (Amount == 0)
            {
                if (Payments == null || !Payments.Any())
                    return "no payment needed";

                throw new InvalidOperationException("The invoice is in an invalid state, it has an amount of 0 and it has payments.");
            }

            var totalPaid = Payments.Sum(x => x.Amount);

            if (totalPaid != 0 && totalPaid == Amount)
                return "invoice was already fully paid";

            if (totalPaid != 0 && payment.Amount > (Amount - totalPaid))
                return "the payment is greater than the partial amount remaining";

            return HandlePartialOrFullPayment(payment);
        }

        private string HandlePartialOrFullPayment(Payment payment)
        {
            if (Amount == payment.Amount)
                return FullyPayInvoice(payment);

            if (Amount < payment.Amount)
                return "the payment is greater than the invoice amount";

            if ((Amount - AmountPaid) == payment.Amount)
                return FinalPartialPayment(payment);

            return AnotherPartialPayment(payment);
        }

        private string FullyPayInvoice(Payment payment)
        {
            AddPayment(payment);
            AmountPaid = payment.Amount;
            TaxAmount = payment.Amount * 0.14m;
            Save();
            return "invoice is now fully paid";
        }

        private string FinalPartialPayment(Payment payment)
        {
            AddPayment(payment);
            AmountPaid += payment.Amount;
            TaxAmount += payment.Amount * 0.14m;
            Save();
            return "final partial payment received, invoice is now fully paid";
        }

        private string AnotherPartialPayment(Payment payment)
        {
            AddPayment(payment);
            AmountPaid += payment.Amount;
            TaxAmount += payment.Amount * 0.14m;
            Save();
            return "another partial payment received, still not fully paid";
        }

        private void AddPayment(Payment payment)
        {
            switch (Type)
            {
                case InvoiceType.Standard:
                    Payments.Add(payment);
                    break;
                case InvoiceType.Commercial:
                    Payments.Add(payment);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Save()
        {
            // Save the invoice state to the database
        }
    }

    public enum InvoiceType
    {
        Standard,
        Commercial
    }

    public class Payment
    {
        public decimal Amount { get; set; }
        public string Reference { get; set; }
    }
}
