using RefactorThis.API.Domain.Repositories;
using RefactorThis.API.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RefactorThis.API.Domain.Entities
{
    public class Invoice
    {

        public decimal Amount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal TaxAmount { get; set; }
        public List<Payment> Payments { get; set; } = new List<Payment>();
        public InvoiceType Type { get; set; }

        public string Reference { get; set; }

        public string ProcessPayment(Payment payment)
        {

            if (Amount == 0) // Check if the invoice amount is 0
            {
                if (Payments == null || !Payments.Any())// If no payments have been made, return "no payment needed"
                {
                    return "no payment needed";
                }
                else
                {
                    // If there are payments but the amount is 0, the invoice is in an invalid state
                    throw new InvalidOperationException("The invoice is in an invalid state, it has an amount of 0 and it has payments.");
                }
            }
            // Check if there are any previous payments
            if (Payments != null && Payments.Any())
            {
                var totalPayments = Payments.Sum(x => x.Amount); // Calculate the total amount of previous payments

                // If the total payments match the invoice amount, it means the invoice is already full paid
                if (totalPayments != 0 && Amount == totalPayments)
                {
                    return "invoice was already fully paid";
                }
                else if (totalPayments != 0 && payment.Amount > (Amount - AmountPaid))
                {
                    return "the payment is greater than the partial amount remaining";
                }
                else
                {
                    // Handle the partial payment scenario
                    return HandlePartialPayment(payment); // Created new method to make it clean and understandable
                }
            }
            else
            {
                return HandleNoPreviousPayments(payment); // If there are no previous payments, handle accordingly
            }
        }
        private string HandlePartialPayment(Payment payment)
        {
            // If the payment amount exactly matches the remaining amount, mark the invoice as fully paid
            if ((Amount - AmountPaid) == payment.Amount)
            {
                ApplyPayment(payment);
                return "final partial payment received, invoice is now fully paid";
            }
            else
            {
                // Otherwise, add the payment and mark the invoice as partially paid
                ApplyPayment(payment);
                return "another partial payment received, still not fully paid";
            }
        }
        private string HandleNoPreviousPayments(Payment payment)
        {
            // If the payment is greater than the invoice amount, return message
            if (payment.Amount > Amount)
            {
                return "the payment is greater than the invoice amount";
            }
            // If the payment exactly matches the invoice amount, mark the invoice as fully paid
            else if (Amount == payment.Amount)
            {
                ApplyPayment(payment);
                return "invoice is now fully paid";
            }
            else
            {
                // Otherwise, add the payment and mark the invoice as partially paid
                ApplyPayment(payment);
                return "invoice is now partially paid";
            }
        }
        private void ApplyPayment(Payment payment)
        {
            AmountPaid += payment.Amount; // Add the payment amount to the total amount paid

            switch (Type) // Handle different invoice types
            {
                case InvoiceType.Standard:
                    // Add the payment to the list of payments for standard invoices
                    Payments.Add(payment);
                    break;
                case InvoiceType.Commercial:
                    // Calculate and add the tax amount for commercial invoices
                    TaxAmount += payment.Amount * 0.14m;
                    Payments.Add(payment);
                    break;
                default:
                    // Throw an exception
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
