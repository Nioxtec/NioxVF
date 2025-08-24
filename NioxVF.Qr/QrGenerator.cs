namespace NioxVF.Qr;

/// <summary>
/// Generador de códigos QR placeholder
/// En la implementación final se usará QRCoder
/// </summary>
public class QrGenerator
{
    /// <summary>
    /// Genera un código QR como array de bytes PNG
    /// </summary>
    /// <param name="data">Datos a codificar</param>
    /// <param name="size">Tamaño del QR (por defecto 200x200)</param>
    /// <returns>Bytes del PNG generado</returns>
    public static byte[] GenerateQrBytes(string data, int size = 200)
    {
        // TODO: Implementar con QRCoder en sprints futuros
        // Por ahora retornamos un placeholder
        
        var imageData = new List<byte>();
        
        // Header PNG básico
        var pngSignature = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        imageData.AddRange(pngSignature);
        
        // Simular datos del QR basado en el hash del contenido
        var hash = data.GetHashCode();
        var simulatedData = new byte[size * 4]; // 4 bytes por pixel (RGBA)
        
        for (int i = 0; i < simulatedData.Length; i++)
        {
            simulatedData[i] = (byte)((hash + i) % 256);
        }
        
        imageData.AddRange(simulatedData);
        
        return imageData.ToArray();
    }
    
    /// <summary>
    /// Genera un código QR y lo guarda en un archivo
    /// </summary>
    /// <param name="data">Datos a codificar</param>
    /// <param name="filePath">Ruta del archivo a crear</param>
    /// <param name="size">Tamaño del QR</param>
    public static void GenerateQrFile(string data, string filePath, int size = 200)
    {
        var qrBytes = GenerateQrBytes(data, size);
        File.WriteAllBytes(filePath, qrBytes);
    }
}
