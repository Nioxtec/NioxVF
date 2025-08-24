using Microsoft.Extensions.Logging;
using NioxVF.Domain.Interfaces;
using NioxVF.Domain.Models;

namespace NioxVF.AeatSoap;

/// <summary>
/// Implementación placeholder del transporte SOAP hacia AEAT
/// En Sprint 4 se implementará el cliente SOAP real con mTLS
/// </summary>
public class AeatSoapTransport : IVeriFactuTransport
{
    private readonly ILogger<AeatSoapTransport>? _logger;

    public AeatSoapTransport(ILogger<AeatSoapTransport>? logger = null)
    {
        _logger = logger;
    }

    public Task<SendResult> SendInvoiceAsync(string signedXml, InvoiceSimple invoice)
    {
        // TODO: Implementar cliente SOAP real con mTLS en Sprint 4
        // Por ahora simulamos una respuesta exitosa de AEAT
        
        _logger?.LogInformation("Simulating AEAT SOAP call for invoice {Series}-{Number}", 
            invoice.Series, invoice.Number);

        var result = new SendResult
        {
            Id = Guid.NewGuid().ToString(),
            Status = "SENT",
            AeatId = $"AEAT-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}",
            ValidationUrl = GenerateValidationUrl(invoice),
            ResponseXml = GenerateSimulatedResponse(invoice),
            Timestamp = DateTime.UtcNow
        };

        _logger?.LogInformation("Simulated AEAT response: {AeatId}", result.AeatId);
        
        return Task.FromResult(result);
    }

    public Task<SendResult> CancelInvoiceAsync(string aeatId, string reason)
    {
        // TODO: Implementar anulación real en AEAT
        
        _logger?.LogInformation("Simulating AEAT cancellation for {AeatId}: {Reason}", aeatId, reason);

        var result = new SendResult
        {
            Id = Guid.NewGuid().ToString(),
            Status = "CANCELLED",
            AeatId = aeatId,
            ResponseXml = GenerateSimulatedCancellationResponse(aeatId, reason),
            Timestamp = DateTime.UtcNow
        };

        return Task.FromResult(result);
    }

    private static string GenerateValidationUrl(InvoiceSimple invoice)
    {
        // Generar URL de validación simulada
        var hash = $"{invoice.SellerNif}{invoice.Series}{invoice.Number}".GetHashCode();
        return $"https://sede.agenciatributaria.gob.es/ValidarQR?nif={invoice.SellerNif}&ser={invoice.Series}&num={invoice.Number}&hash={hash:X}";
    }

    private static string GenerateSimulatedResponse(InvoiceSimple invoice)
    {
        return $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
    <soap:Body>
        <RegistroFacturaResponse xmlns=""https://www2.agenciatributaria.gob.es/static_files/common/internet/dep/aplicaciones/es/aeat/tike/cont/ws/RegistroFactura.xsd"">
            <RespuestaLinea>
                <IDFactura>
                    <IDEmisorFactura>
                        <NIF>{invoice.SellerNif}</NIF>
                    </IDEmisorFactura>
                    <NumSerieFactura>{invoice.Series}</NumSerieFactura>
                    <NumFactura>{invoice.Number}</NumFactura>
                    <FechaExpedicionFactura>{invoice.IssueDate:yyyy-MM-dd}</FechaExpedicionFactura>
                </IDFactura>
                <EstadoRegistro>Correcta</EstadoRegistro>
                <CodigoErrorRegistro>0</CodigoErrorRegistro>
                <DescripcionErrorRegistro></DescripcionErrorRegistro>
                <CSV>AEAT-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}</CSV>
            </RespuestaLinea>
        </RegistroFacturaResponse>
    </soap:Body>
</soap:Envelope>";
    }

    private static string GenerateSimulatedCancellationResponse(string aeatId, string reason)
    {
        return $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
    <soap:Body>
        <AnulacionFacturaResponse xmlns=""https://www2.agenciatributaria.gob.es/static_files/common/internet/dep/aplicaciones/es/aeat/tike/cont/ws/RegistroFactura.xsd"">
            <RespuestaLinea>
                <CSV>{aeatId}</CSV>
                <EstadoRegistro>Anulada</EstadoRegistro>
                <CodigoErrorRegistro>0</CodigoErrorRegistro>
                <DescripcionErrorRegistro></DescripcionErrorRegistro>
                <MotivoAnulacion>{reason}</MotivoAnulacion>
            </RespuestaLinea>
        </AnulacionFacturaResponse>
    </soap:Body>
</soap:Envelope>";
    }
}
