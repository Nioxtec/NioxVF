using Microsoft.AspNetCore.Mvc;
using NioxVF.Api.DTOs;
using NioxVF.Api.Services;
using NioxVF.Domain.Enums;
using NioxVF.Domain.Interfaces;

namespace NioxVF.Api.Controllers;

/// <summary>
/// Controlador para gestión de facturas Veri*Factu
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;
    private readonly IHashChain _hashChain;
    private readonly ILogger<InvoicesController> _logger;

    public InvoicesController(
        IInvoiceService invoiceService,
        IHashChain hashChain,
        ILogger<InvoicesController> logger)
    {
        _invoiceService = invoiceService;
        _hashChain = hashChain;
        _logger = logger;
    }

    /// <summary>
    /// Procesa una nueva factura
    /// </summary>
    /// <param name="request">Datos de la factura y modo de procesamiento</param>
    /// <returns>Resultado del procesamiento</returns>
    [HttpPost]
    public async Task<ActionResult<InvoiceResponse>> ProcessInvoice([FromBody] InvoiceRequest request)
    {
        try
        {
            _logger.LogInformation("Processing invoice {Series}-{Number} for seller {SellerNif}", 
                request.Invoice.Series, request.Invoice.Number, request.Invoice.SellerNif);

            // Validar modo
            if (request.Mode != "sign-and-send" && request.Mode != "send-signed")
            {
                return BadRequest(new { error = "Invalid mode. Must be 'sign-and-send' or 'send-signed'" });
            }

            // Validar XML firmado si es modo send-signed
            if (request.Mode == "send-signed" && string.IsNullOrEmpty(request.XmlSignedBase64))
            {
                return BadRequest(new { error = "XmlSignedBase64 is required for mode 'send-signed'" });
            }

            // Calcular encadenado
            var prevHash = await _hashChain.GetPreviousHashAsync(request.Invoice.SellerNif, request.Invoice.Series);
            request.Invoice.PrevHash = prevHash;
            request.Invoice.Hash = _hashChain.CalculateHash(request.Invoice);

            // Procesar factura
            var result = await _invoiceService.ProcessInvoiceAsync(request.Invoice, request.Mode, request.XmlSignedBase64);

            // Actualizar cadena de hash
            await _hashChain.UpdateChainAsync(request.Invoice.SellerNif, request.Invoice.Series, prevHash, request.Invoice.Hash);

            var response = new InvoiceResponse
            {
                Id = result.Id,
                Status = result.Status,
                AeatId = result.AeatId,
                QrUrl = $"/api/v1/invoices/{result.Id}/qr",
                ValidationUrl = result.ValidationUrl,
                ErrorCode = result.ErrorCode,
                ErrorMessage = result.ErrorMessage
            };

            _logger.LogInformation("Invoice {Id} processed with status {Status}", result.Id, result.Status);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing invoice {Series}-{Number}", 
                request.Invoice.Series, request.Invoice.Number);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Obtiene el detalle de una factura
    /// </summary>
    /// <param name="id">ID de la factura</param>
    /// <returns>Detalle de la factura</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<InvoiceResponse>> GetInvoice(string id)
    {
        try
        {
            var invoice = await _invoiceService.GetInvoiceAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            var response = new InvoiceResponse
            {
                Id = invoice.Id,
                Status = invoice.Status,
                AeatId = invoice.AeatId,
                QrUrl = $"/api/v1/invoices/{invoice.Id}/qr",
                ValidationUrl = invoice.ValidationUrl
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving invoice {Id}", id);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Obtiene el QR de una factura
    /// </summary>
    /// <param name="id">ID de la factura</param>
    /// <returns>Imagen QR en formato PNG</returns>
    [HttpGet("{id}/qr")]
    public async Task<IActionResult> GetInvoiceQr(string id)
    {
        try
        {
            var qrBytes = await _invoiceService.GetInvoiceQrAsync(id);
            if (qrBytes == null)
            {
                return NotFound();
            }

            return File(qrBytes, "image/png", $"qr-{id}.png");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving QR for invoice {Id}", id);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Anula una factura
    /// </summary>
    /// <param name="id">ID de la factura</param>
    /// <param name="request">Datos de anulación</param>
    /// <returns>Resultado de la anulación</returns>
    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<InvoiceResponse>> CancelInvoice(string id, [FromBody] CancelRequest request)
    {
        try
        {
            var result = await _invoiceService.CancelInvoiceAsync(id, request.Reason);
            if (result == null)
            {
                return NotFound();
            }

            var response = new InvoiceResponse
            {
                Id = result.Id,
                Status = result.Status,
                AeatId = result.AeatId,
                ValidationUrl = result.ValidationUrl
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling invoice {Id}", id);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}

/// <summary>
/// DTO para solicitud de anulación
/// </summary>
public class CancelRequest
{
    public required string Reason { get; set; }
}
