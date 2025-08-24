namespace NioxVF.Api.DTOs;

/// <summary>
/// DTO para respuesta de procesamiento de factura
/// </summary>
public class InvoiceResponse
{
    /// <summary>
    /// ID único de la factura en el sistema
    /// </summary>
    public required string Id { get; set; }
    
    /// <summary>
    /// Estado actual: QUEUED, SENT, ACCEPTED, REJECTED, ERROR
    /// </summary>
    public required string Status { get; set; }
    
    /// <summary>
    /// ID asignado por AEAT (si está disponible)
    /// </summary>
    public string? AeatId { get; set; }
    
    /// <summary>
    /// URL para obtener el QR de la factura
    /// </summary>
    public string? QrUrl { get; set; }
    
    /// <summary>
    /// URL de validación en AEAT
    /// </summary>
    public string? ValidationUrl { get; set; }
    
    /// <summary>
    /// Código de error (si aplica)
    /// </summary>
    public string? ErrorCode { get; set; }
    
    /// <summary>
    /// Mensaje de error (si aplica)
    /// </summary>
    public string? ErrorMessage { get; set; }
}
