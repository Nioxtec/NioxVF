using NioxVF.Domain.Models;

namespace NioxVF.Api.DTOs;

/// <summary>
/// DTO para solicitud de procesamiento de factura
/// </summary>
public class InvoiceRequest
{
    /// <summary>
    /// Datos de la factura
    /// </summary>
    public required InvoiceSimple Invoice { get; set; }
    
    /// <summary>
    /// Modo de procesamiento: "sign-and-send" | "send-signed"
    /// </summary>
    public required string Mode { get; set; }
    
    /// <summary>
    /// XML firmado en base64 (solo para modo "send-signed")
    /// </summary>
    public string? XmlSignedBase64 { get; set; }
}
