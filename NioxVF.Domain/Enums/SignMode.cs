namespace NioxVF.Domain.Enums;

/// <summary>
/// Modos de firma disponibles
/// </summary>
public enum SignMode
{
    /// <summary>
    /// Firmar en el Agent local y enviar firmado
    /// </summary>
    Local,
    
    /// <summary>
    /// Enviar datos sin firmar para firma en la API
    /// </summary>
    Cloud
}
