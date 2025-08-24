using NioxVF.Domain.Models;

namespace NioxVF.Api.Services;

/// <summary>
/// DTO para almacenar datos de factura procesada
/// </summary>
public class ProcessedInvoice
{
    public required string Id { get; set; }
    public required string Status { get; set; }
    public string? AeatId { get; set; }
    public string? ValidationUrl { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Interfaz del servicio de gestión de facturas
/// </summary>
public interface IInvoiceService
{
    /// <summary>
    /// Procesa una factura según el modo especificado
    /// </summary>
    /// <param name="invoice">Datos de la factura</param>
    /// <param name="mode">Modo de procesamiento</param>
    /// <param name="xmlSignedBase64">XML firmado (si aplica)</param>
    /// <returns>Resultado del procesamiento</returns>
    Task<ProcessedInvoice> ProcessInvoiceAsync(InvoiceSimple invoice, string mode, string? xmlSignedBase64);
    
    /// <summary>
    /// Obtiene una factura por ID
    /// </summary>
    /// <param name="id">ID de la factura</param>
    /// <returns>Datos de la factura o null si no existe</returns>
    Task<ProcessedInvoice?> GetInvoiceAsync(string id);
    
    /// <summary>
    /// Obtiene el QR de una factura
    /// </summary>
    /// <param name="id">ID de la factura</param>
    /// <returns>Bytes del PNG del QR o null si no existe</returns>
    Task<byte[]?> GetInvoiceQrAsync(string id);
    
    /// <summary>
    /// Anula una factura
    /// </summary>
    /// <param name="id">ID de la factura</param>
    /// <param name="reason">Motivo de anulación</param>
    /// <returns>Resultado de la anulación o null si no existe</returns>
    Task<ProcessedInvoice?> CancelInvoiceAsync(string id, string reason);
}
