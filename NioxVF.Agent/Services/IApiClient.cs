using NioxVF.Domain.Models;

namespace NioxVF.Agent.Services;

/// <summary>
/// DTO para respuesta de la API
/// </summary>
public class InvoiceResponse
{
    public required string Id { get; set; }
    public required string Status { get; set; }
    public string? AeatId { get; set; }
    public string? QrUrl { get; set; }
    public string? ValidationUrl { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Interfaz para cliente de la API NioxVF
/// </summary>
public interface IApiClient
{
    /// <summary>
    /// Envía una factura a la API para procesamiento
    /// </summary>
    /// <param name="invoice">Datos de la factura</param>
    /// <param name="mode">Modo de procesamiento</param>
    /// <param name="xmlSignedBase64">XML firmado (opcional)</param>
    /// <returns>Respuesta de la API</returns>
    Task<InvoiceResponse?> ProcessInvoiceAsync(InvoiceSimple invoice, string mode, string? xmlSignedBase64 = null);
    
    /// <summary>
    /// Obtiene el estado de una factura
    /// </summary>
    /// <param name="invoiceId">ID de la factura</param>
    /// <returns>Estado de la factura</returns>
    Task<InvoiceResponse?> GetInvoiceStatusAsync(string invoiceId);
}
