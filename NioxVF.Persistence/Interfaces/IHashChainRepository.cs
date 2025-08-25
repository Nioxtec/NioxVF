using NioxVF.Persistence.Entities;

namespace NioxVF.Persistence.Interfaces;

/// <summary>
/// Interfaz para el repositorio de cadenas de hashes
/// </summary>
public interface IHashChainRepository : IRepository<HashChainEntity>
{
    /// <summary>
    /// Obtiene el hash anterior para una serie específica de un vendedor
    /// </summary>
    /// <param name="sellerNif">NIF del vendedor</param>
    /// <param name="series">Serie de facturación</param>
    /// <returns>El hash anterior o null si es la primera factura</returns>
    Task<string?> GetPreviousHashAsync(string sellerNif, string series);

    /// <summary>
    /// Actualiza la cadena de hashes para una serie específica
    /// </summary>
    /// <param name="sellerNif">NIF del vendedor</param>
    /// <param name="series">Serie de facturación</param>
    /// <param name="prevHash">Hash anterior</param>
    /// <param name="currentHash">Hash actual</param>
    Task UpdateChainAsync(string sellerNif, string series, string? prevHash, string currentHash);

    /// <summary>
    /// Obtiene todas las cadenas de hashes de un vendedor
    /// </summary>
    /// <param name="sellerNif">NIF del vendedor</param>
    /// <returns>Colección de cadenas de hashes del vendedor</returns>
    Task<IEnumerable<HashChainEntity>> GetChainBySellerAsync(string sellerNif);
}
