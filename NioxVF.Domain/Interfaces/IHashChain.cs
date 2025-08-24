using NioxVF.Domain.Models;

namespace NioxVF.Domain.Interfaces;

/// <summary>
/// Interfaz para el servicio de encadenado de documentos Veri*Factu
/// </summary>
public interface IHashChain
{
    /// <summary>
    /// Calcula el hash previo para una factura en base al seller y serie
    /// </summary>
    /// <param name="sellerNif">NIF del obligado tributario</param>
    /// <param name="series">Serie del documento</param>
    /// <returns>Hash del documento anterior o null si es el primero</returns>
    Task<string?> GetPreviousHashAsync(string sellerNif, string series);
    
    /// <summary>
    /// Calcula el hash de una factura
    /// </summary>
    /// <param name="invoice">Factura a procesar</param>
    /// <returns>Hash SHA-256 en mayúsculas</returns>
    string CalculateHash(InvoiceSimple invoice);
    
    /// <summary>
    /// Actualiza la cadena con un nuevo documento
    /// </summary>
    /// <param name="sellerNif">NIF del obligado tributario</param>
    /// <param name="series">Serie del documento</param>
    /// <param name="prevHash">Hash anterior</param>
    /// <param name="currentHash">Hash actual</param>
    Task UpdateChainAsync(string sellerNif, string series, string? prevHash, string currentHash);
}
