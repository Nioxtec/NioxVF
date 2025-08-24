namespace NioxVF.Agent.Services;

/// <summary>
/// Interfaz para generación de códigos QR
/// </summary>
public interface IQrGenerator
{
    /// <summary>
    /// Genera un código QR y lo guarda como imagen PNG
    /// </summary>
    /// <param name="data">Datos a codificar en el QR</param>
    /// <param name="filePath">Ruta donde guardar el archivo PNG</param>
    Task GenerateQrAsync(string data, string filePath);
    
    /// <summary>
    /// Genera un código QR como array de bytes PNG
    /// </summary>
    /// <param name="data">Datos a codificar en el QR</param>
    /// <returns>Bytes del PNG generado</returns>
    Task<byte[]> GenerateQrBytesAsync(string data);
}
