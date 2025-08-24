namespace NioxVF.Agent.Services;

/// <summary>
/// Generador de QR placeholder para MVP (sin dependencia externa)
/// </summary>
public class PlaceholderQrGenerator : IQrGenerator
{
    private readonly ILogger<PlaceholderQrGenerator> _logger;

    public PlaceholderQrGenerator(ILogger<PlaceholderQrGenerator> logger)
    {
        _logger = logger;
    }

    public async Task GenerateQrAsync(string data, string filePath)
    {
        var qrBytes = await GenerateQrBytesAsync(data);
        await File.WriteAllBytesAsync(filePath, qrBytes);
        
        _logger.LogInformation("Placeholder QR generated at {FilePath} for data: {Data}", filePath, data);
    }

    public Task<byte[]> GenerateQrBytesAsync(string data)
    {
        // Generar un PNG placeholder simple de 200x200 píxeles
        // En la implementación real se usaría QRCoder
        
        // Crear un bitmap básico simulando un QR
        var imageData = new List<byte>();
        
        // Header PNG básico (simplificado)
        var pngSignature = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        imageData.AddRange(pngSignature);
        
        // Datos simulados del PNG (normalmente sería un PNG real)
        // Por simplicidad, generamos un patrón que simula un QR
        var simulatedPngData = new byte[2000]; // Datos del PNG simulado
        
        // Llenar con un patrón que simule un QR
        for (int i = 0; i < simulatedPngData.Length; i++)
        {
            // Crear un patrón pseudo-aleatorio basado en el hash del data
            var hash = data.GetHashCode();
            simulatedPngData[i] = (byte)((hash + i) % 256);
        }
        
        imageData.AddRange(simulatedPngData);
        
        _logger.LogDebug("Generated placeholder QR bytes for data: {Data}", data);
        
        return Task.FromResult(imageData.ToArray());
    }
}
