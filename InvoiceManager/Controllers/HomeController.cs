using InvoiceManager.Models.Domains;
using InvoiceManager.Models.Repositories;
using InvoiceManager.Models.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InvoiceManager.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private InvoiceRepository _invoiceRepository = new InvoiceRepository();
        private ClientRepository _clientRepository = new ClientRepository();
        private ProductRepository _productRepository = new ProductRepository();
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var invoices = _invoiceRepository.GetInvoices(userId);
            return View(invoices);
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Invoice(int invoiceId = 0)
        {
            var userId = User.Identity.GetUserId();
            var invoice = (invoiceId == 0) ? GetNewInvoice(userId) : _invoiceRepository.GetInvoice(invoiceId, userId);

            var vm = PrepareInvoiceVm(invoice, userId);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Invoice(Invoice invoice)
        {
            var userId = User.Identity.GetUserId();
            invoice.UserId = userId;

            if(!ModelState.IsValid)
            {
                var vm = PrepareInvoiceVm(invoice, userId);
                return View("Invoice", vm);
            }

            if (invoice.Id == 0)
                _invoiceRepository.AddInvoice(invoice);
            else
                _invoiceRepository.UpdateInvoice(invoice);

            return RedirectToAction("Index");
        }

        private EditInvoiceViewModel PrepareInvoiceVm(Invoice invoice, string userId)
        {
            return new EditInvoiceViewModel
            {
                Invoice = invoice,
                Header = (invoice.Id == 0) ? "Dodawanie faktury" : "Edycja faktury",
                Clients = _clientRepository.GetClients(userId),
                MethodOfPayments = _invoiceRepository.GetMethodOfPayments()
            };
        }

        private Invoice GetNewInvoice(string userId)
        {
            return new Invoice
            {
                UserId = userId,
                PaymentDate = DateTime.Now.AddDays(7),
                CreatedDate = DateTime.Now
            };
        }

        public ActionResult InvoicePosition(int invoiceId, int invoicePositionId = 0)
        {
            var userId = User.Identity.GetUserId();
            var invoicePosition = (invoicePositionId == 0) ? GetNewInvoicePosition(invoiceId) : _invoiceRepository.GetInvoicePosition(invoiceId, invoicePositionId, userId);

            var vm = PrepareInvoicePosition(invoicePosition);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InvoicePosition(InvoicePosition invoicePosition)
        {
            if (!ModelState.IsValid)
            {
                var vm = PrepareInvoicePosition(invoicePosition);
                return View("InvoicePosition", vm);
            }

            var userId = User.Identity.GetUserId();
            var product = _productRepository.GetProduct(invoicePosition.ProductId);

            invoicePosition.Value = invoicePosition.Quantity * product.Value;

            if (invoicePosition.Id == 0)
                _invoiceRepository.AddInvoicePosition(invoicePosition, userId);
            else
                _invoiceRepository.UpdateInvoicePosition(invoicePosition, userId);

            _invoiceRepository.UpdateInvoiceValue(invoicePosition.InvoiceId, userId);

            return RedirectToAction("Invoice", new { invoiceId = invoicePosition.InvoiceId });
        }

        private EditInvoicePositionViewModel PrepareInvoicePosition(InvoicePosition invoicePosition)
        {
            return new EditInvoicePositionViewModel
            {
                InvoicePosition = invoicePosition,
                Header = (invoicePosition.Id == 0) ? "Dodawanie pozycji" : "Edycja pozycji",
                Products = _productRepository.GetProducts()
            };

        }

        private InvoicePosition GetNewInvoicePosition(int invoiceId)
        {
            return new InvoicePosition
            {
                InvoiceId = invoiceId,
                Id = 0
            };
        }

        [HttpPost]
        public ActionResult DeleteInvoice(int invoiceId)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                _invoiceRepository.RemoveInvoice(invoiceId, userId);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
            
            return Json(new { Success = true });
        }

        [HttpPost]
        public ActionResult DeleteInvoicePosition(int invoicePositionId, int invoiceId)
        {
            var invoiceValue = 0m;

            try
            {
                var userId = User.Identity.GetUserId();
                _invoiceRepository.RemoveInvoicePosition(invoicePositionId, invoiceId, userId);
                invoiceValue = _invoiceRepository.UpdateInvoiceValue(invoiceId, userId);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }

            return Json(new { Success = true, InvoiceValue = invoiceValue });
        }
    }
}