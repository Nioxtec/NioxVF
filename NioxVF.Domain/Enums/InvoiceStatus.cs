namespace NioxVF.Domain.Enums;

/// <summary>
/// Estados posibles de una factura en el sistema
/// </summary>
public enum InvoiceStatus
{
    /// <summary>
    /// En cola para procesamiento
    /// </summary>
    Queued,
    
    /// <summary>
    /// Enviada a AEAT
    /// </summary>
    Sent,
    
    /// <summary>
    /// Aceptada por AEAT
    /// </summary>
    Accepted,
    
    /// <summary>
    /// Rechazada por AEAT
    /// </summary>
    Rejected,
    
    /// <summary>
    /// Error en el procesamiento
    /// </summary>
    Error,
    
    /// <summary>
    /// Anulada
    /// </summary>
    Cancelled
}
