using System;
using RefactorThis.API.Domain.Entities;
using RefactorThis.API.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace RefactorThis.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly InvoiceService _invoiceService;

        public InvoiceController(InvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpPost("process-payment")]
        public IActionResult ProcessPayment([FromBody] Payment payment)
        {
            try
            {
                var result = _invoiceService.ProcessPayment(payment);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
