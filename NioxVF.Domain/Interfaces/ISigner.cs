namespace NioxVF.Domain.Interfaces;

/// <summary>
/// Interfaz para el servicio de firma XAdES-EPES
/// </summary>
public interface ISigner
{
    /// <summary>
    /// Firma un documento XML con XAdES-EPES
    /// </summary>
    /// <param name="xmlContent">Contenido XML a firmar</param>
    /// <param name="certificatePath">Ruta al certificado (PFX) o thumbprint si es del almacén</param>
    /// <param name="certificatePassword">Contraseña del certificado (si es PFX)</param>
    /// <returns>XML firmado en formato base64</returns>
    Task<string> SignXmlAsync(string xmlContent, string certificatePath, string? certificatePassword = null);
    
    /// <summary>
    /// Valida una firma XAdES-EPES
    /// </summary>
    /// <param name="signedXml">XML firmado</param>
    /// <returns>True si la firma es válida</returns>
    Task<bool> ValidateSignatureAsync(string signedXml);
}
