using NioxVF.Domain.Models;
using NioxVF.Domain.Interfaces;
using System.Collections.Concurrent;

namespace NioxVF.Api.Services;

/// <summary>
/// Implementación en memoria del servicio de facturas (MVP)
/// </summary>
public class InvoiceService : IInvoiceService
{
    private readonly ConcurrentDictionary<string, ProcessedInvoice> _invoices = new();
    private readonly ILogger<InvoiceService> _logger;
    private readonly ISigner? _signer;
    private readonly IVeriFactuTransport? _transport;

    public InvoiceService(ILogger<InvoiceService> logger, ISigner? signer = null, IVeriFactuTransport? transport = null)
    {
        _logger = logger;
        _signer = signer;
        _transport = transport;
    }

    public async Task<ProcessedInvoice> ProcessInvoiceAsync(InvoiceSimple invoice, string mode, string? xmlSignedBase64)
    {
        var id = Guid.NewGuid().ToString();
        
        try
        {
            _logger.LogInformation("Processing invoice {Id} in mode {Mode}", id, mode);

            // Validar factura
            ValidateInvoice(invoice);

            // Generar XML
            var xmlContent = GenerateXmlF1(invoice);
            
            string signedXml;
            
            if (mode == "send-signed")
            {
                if (string.IsNullOrEmpty(xmlSignedBase64))
                {
                    throw new ArgumentException("XmlSignedBase64 is required for send-signed mode");
                }
                
                signedXml = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(xmlSignedBase64));
                _logger.LogDebug("Using pre-signed XML for invoice {Id}", id);
            }
            else // sign-and-send
            {
                if (_signer != null)
                {
                    // TODO: Implementar firma real cuando esté disponible
                    signedXml = $"<SignedXML>{xmlContent}</SignedXML>";
                    _logger.LogDebug("Signed XML for invoice {Id}", id);
                }
                else
                {
                    // Placeholder para MVP sin firma real
                    signedXml = $"<PlaceholderSignedXML>{xmlContent}</PlaceholderSignedXML>";
                    _logger.LogDebug("Using placeholder signature for invoice {Id}", id);
                }
            }

            // Enviar a AEAT (placeholder para MVP)
            string status;
            string? aeatId = null;
            string? validationUrl = null;

            if (_transport != null)
            {
                var sendResult = await _transport.SendInvoiceAsync(signedXml, invoice);
                status = sendResult.Status;
                aeatId = sendResult.AeatId;
                validationUrl = sendResult.ValidationUrl;
            }
            else
            {
                // Simulación para MVP
                status = "SENT";
                aeatId = $"AEAT-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}";
                validationUrl = $"https://sede.agenciatributaria.gob.es/ValidarQR?id={aeatId}";
                _logger.LogDebug("Simulated AEAT response for invoice {Id}: {AeatId}", id, aeatId);
            }

            var processedInvoice = new ProcessedInvoice
            {
                Id = id,
                Status = status,
                AeatId = aeatId,
                ValidationUrl = validationUrl
            };

            _invoices[id] = processedInvoice;
            
            _logger.LogInformation("Invoice {Id} processed successfully with AEAT ID {AeatId}", id, aeatId);
            return processedInvoice;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing invoice {Id}", id);
            
            var errorInvoice = new ProcessedInvoice
            {
                Id = id,
                Status = "ERROR",
                ErrorCode = "PROCESSING_ERROR",
                ErrorMessage = ex.Message
            };

            _invoices[id] = errorInvoice;
            return errorInvoice;
        }
    }

    public Task<ProcessedInvoice?> GetInvoiceAsync(string id)
    {
        _invoices.TryGetValue(id, out var invoice);
        return Task.FromResult(invoice);
    }

    public async Task<byte[]?> GetInvoiceQrAsync(string id)
    {
        var invoice = await GetInvoiceAsync(id);
        if (invoice?.AeatId == null)
        {
            return null;
        }

        // Generar QR placeholder
        // TODO: Implementar generación real de QR cuando esté disponible
        var qrData = $"https://sede.agenciatributaria.gob.es/ValidarQR?id={invoice.AeatId}";
        var qrPlaceholder = GenerateQrPlaceholder(qrData);
        
        return qrPlaceholder;
    }

    public async Task<ProcessedInvoice?> CancelInvoiceAsync(string id, string reason)
    {
        var invoice = await GetInvoiceAsync(id);
        if (invoice == null)
        {
            return null;
        }

        // TODO: Implementar anulación real en AEAT
        invoice.Status = "CANCELLED";
        _logger.LogInformation("Invoice {Id} cancelled: {Reason}", id, reason);
        
        return invoice;
    }

    private static void ValidateInvoice(InvoiceSimple invoice)
    {
        if (string.IsNullOrWhiteSpace(invoice.SellerNif))
            throw new ArgumentException("SellerNif is required");
        
        if (string.IsNullOrWhiteSpace(invoice.Series))
            throw new ArgumentException("Series is required");
        
        if (string.IsNullOrWhiteSpace(invoice.Number))
            throw new ArgumentException("Number is required");
        
        if (invoice.Taxes == null || !invoice.Taxes.Any())
            throw new ArgumentException("At least one tax item is required");

        // Validar que las sumas cuadren
        foreach (var tax in invoice.Taxes)
        {
            var expectedTaxAmount = Math.Round(tax.TaxBase * tax.TaxRate / 100, 2);
            if (Math.Abs(tax.TaxAmount - expectedTaxAmount) > 0.01m)
            {
                throw new ArgumentException($"Tax amount mismatch for rate {tax.TaxRate}%");
            }
        }
    }

    private static string GenerateXmlF1(InvoiceSimple invoice)
    {
        // Generación básica de XML F1 (placeholder para MVP)
        return $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<FacturaSimplificada>
    <SellerNif>{invoice.SellerNif}</SellerNif>
    <SellerName>{invoice.SellerName}</SellerName>
    <Series>{invoice.Series}</Series>
    <Number>{invoice.Number}</Number>
    <IssueDate>{invoice.IssueDate:yyyy-MM-dd}</IssueDate>
    <Total>{invoice.Total:F2}</Total>
    <Hash>{invoice.Hash}</Hash>
    <PrevHash>{invoice.PrevHash ?? ""}</PrevHash>
    <Taxes>
        {string.Join("", invoice.Taxes.Select(t => $@"
        <Tax>
            <TaxBase>{t.TaxBase:F2}</TaxBase>
            <TaxRate>{t.TaxRate:F1}</TaxRate>
            <TaxAmount>{t.TaxAmount:F2}</TaxAmount>
        </Tax>"))}
    </Taxes>
</FacturaSimplificada>";
    }

    private static byte[] GenerateQrPlaceholder(string data)
    {
        // Generar un PNG placeholder simple de 200x200 píxeles
        // En la implementación real se usaría QRCoder
        
        var width = 200;
        var height = 200;
        var bytesPerPixel = 4; // RGBA
        var totalBytes = width * height * bytesPerPixel;
        
        var imageData = new byte[totalBytes];
        
        // Crear un patrón simple de cuadrícula para simular QR
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var index = (y * width + x) * bytesPerPixel;
                
                // Crear patrón de cuadrícula
                var isBlack = (x / 10 + y / 10) % 2 == 0;
                
                if (isBlack)
                {
                    imageData[index] = 0;     // R
                    imageData[index + 1] = 0; // G
                    imageData[index + 2] = 0; // B
                }
                else
                {
                    imageData[index] = 255;     // R
                    imageData[index + 1] = 255; // G
                    imageData[index + 2] = 255; // B
                }
                imageData[index + 3] = 255; // Alpha
            }
        }

        // Header PNG básico + datos
        var pngSignature = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        
        // Por simplicidad, devolvemos datos binarios simulados
        var result = new byte[1000]; // PNG placeholder pequeño
        Array.Copy(pngSignature, result, pngSignature.Length);
        
        return result;
    }
}
