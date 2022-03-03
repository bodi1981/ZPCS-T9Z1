using InvoiceManager.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;

namespace InvoiceManager.Models.Repositories
{
    public class InvoiceRepository
    {
        public List<Invoice> GetInvoices(string userId)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                return dbContext.Invoices
                    .Include(x => x.Client)
                    .Where(x => x.UserId == userId)
                    .ToList();
            }
        }

        public Invoice GetInvoice(int invoiceId, string userId)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                return dbContext.Invoices
                    .Include(x => x.InvoicePositions)
                    .Include(x => x.InvoicePositions.Select(y => y.Product))
                    .Include(x => x.MethodOfPayment)
                    .Include(x => x.Client)
                    .Include(x => x.Client.Address)
                    .Include(x => x.User)
                    .Include(x => x.User.Address)
                    .Single(x => x.Id == invoiceId && x.UserId == userId);
            }
        }

        public List<MethodOfPayment> GetMethodOfPayments()
        {
            using (var dbContext = new ApplicationDbContext())
            {
                return dbContext.MethodOfPayments.ToList();
            }
        }

        public InvoicePosition GetInvoicePosition(int invoiceId, int invoicePositionId, string userId)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                return dbContext.InvoicePositions
                    .Include(x => x.Invoice)
                    .Single(x => x.Id == invoicePositionId && x.InvoiceId == invoiceId && x.Invoice.UserId == userId);
            }
        }

        public void AddInvoice(Invoice invoice)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                invoice.CreatedDate = DateTime.Now;
                dbContext.Invoices.Add(invoice);
                dbContext.SaveChanges();
            }
        }

        public void UpdateInvoice(Invoice invoice)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var invoiceToUpdate = dbContext.Invoices
                    .Single(x => x.Id == invoice.Id && x.UserId == invoice.UserId);

                invoiceToUpdate.ClientId = invoice.ClientId;
                invoiceToUpdate.Comments = invoice.Comments;
                invoiceToUpdate.MethodOfPaymentId = invoice.MethodOfPaymentId;
                invoiceToUpdate.PaymentDate = invoice.PaymentDate;
                invoiceToUpdate.Title = invoice.Title;

                dbContext.SaveChanges();
            }
        }

        public void AddInvoicePosition(InvoicePosition invoicePosition, string userId)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var invoice = dbContext
                    .Invoices.Single(x => x.Id == invoicePosition.InvoiceId && x.UserId == userId);

                dbContext.InvoicePositions.Add(invoicePosition);
                dbContext.SaveChanges();
            }
        }

        public void UpdateInvoicePosition(InvoicePosition invoicePosition, string userId)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var invoicePositionToUpdate = dbContext.InvoicePositions
                    .Include(x => x.Invoice)
                    .Include(x => x.Product)
                    .Single(x => x.Id == invoicePosition.Id && x.Invoice.UserId == userId);

                invoicePositionToUpdate.No = invoicePosition.No;
                invoicePositionToUpdate.ProductId = invoicePosition.ProductId;
                invoicePositionToUpdate.Quantity = invoicePosition.Quantity;
                invoicePositionToUpdate.Value = invoicePosition.Value * invoicePosition.Product.Value;

                dbContext.SaveChanges();
            }
        }

        public decimal UpdateInvoiceValue(int invoiceId, string userId)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var invoice = dbContext.Invoices
                    .Include(x => x.InvoicePositions)
                    .Single(x => x.Id == invoiceId && x.UserId == userId);

                invoice.Value = invoice.InvoicePositions.Sum(x => x.Value);
                dbContext.SaveChanges();

                return invoice.Value;
            }
        }

        public void RemoveInvoice(int invoiceId, string userId)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var invoiceToRemove = dbContext.Invoices.Single(x => x.Id == invoiceId && x.UserId == userId);

                dbContext.Invoices.Remove(invoiceToRemove);
                dbContext.SaveChanges();
            }
        }

        public void RemoveInvoicePosition(int invoicePositionId, int invoiceId, string userId)
        {
            using (var dbContext = new ApplicationDbContext())
            {
                var invoicePositionToRemove = dbContext.InvoicePositions
                    .Include(x => x.Invoice)
                    .Single(x => x.Id == invoicePositionId && x.Invoice.Id == invoiceId && x.Invoice.UserId == userId);

                dbContext.InvoicePositions.Remove(invoicePositionToRemove);
                dbContext.SaveChanges();
            }
        }
    }
}