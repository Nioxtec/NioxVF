using NioxVF.Domain.Models;

namespace NioxVF.Domain.Interfaces;

/// <summary>
/// Resultado del envío a AEAT
/// </summary>
public class SendResult
{
    public required string Id { get; set; }
    public required string Status { get; set; }
    public string? AeatId { get; set; }
    public string? ValidationUrl { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ResponseXml { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Interfaz para el transporte SOAP con mTLS hacia AEAT
/// </summary>
public interface IVeriFactuTransport
{
    /// <summary>
    /// Envía una factura firmada a AEAT
    /// </summary>
    /// <param name="signedXml">XML firmado</param>
    /// <param name="invoice">Datos de la factura</param>
    /// <returns>Resultado del envío</returns>
    Task<SendResult> SendInvoiceAsync(string signedXml, InvoiceSimple invoice);
    
    /// <summary>
    /// Anula una factura en AEAT
    /// </summary>
    /// <param name="aeatId">ID de AEAT del documento a anular</param>
    /// <param name="reason">Motivo de anulación</param>
    /// <returns>Resultado de la anulación</returns>
    Task<SendResult> CancelInvoiceAsync(string aeatId, string reason);
}
