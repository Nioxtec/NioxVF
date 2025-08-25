using NioxVF.Persistence.Entities;

namespace NioxVF.Persistence.Interfaces;

/// <summary>
/// Interfaz para el repositorio de locks de series para control de concurrencia
/// </summary>
public interface ISeriesLockRepository : IRepository<SeriesLockEntity>
{
    /// <summary>
    /// Intenta adquirir un lock para una serie específica de un vendedor
    /// </summary>
    /// <param name="sellerNif">NIF del vendedor</param>
    /// <param name="series">Serie de facturación</param>
    /// <param name="timeout">Tiempo de espera para adquirir el lock</param>
    /// <returns>True si se adquirió el lock, false en caso contrario</returns>
    Task<bool> TryAcquireLockAsync(string sellerNif, string series, TimeSpan timeout);

    /// <summary>
    /// Libera un lock para una serie específica de un vendedor
    /// </summary>
    /// <param name="sellerNif">NIF del vendedor</param>
    /// <param name="series">Serie de facturación</param>
    Task ReleaseLockAsync(string sellerNif, string series);

    /// <summary>
    /// Verifica si una serie está bloqueada para un vendedor
    /// </summary>
    /// <param name="sellerNif">NIF del vendedor</param>
    /// <param name="series">Serie de facturación</param>
    /// <returns>True si la serie está bloqueada, false en caso contrario</returns>
    Task<bool> IsLockedAsync(string sellerNif, string series);

    /// <summary>
    /// Limpia los locks expirados
    /// </summary>
    Task CleanupExpiredLocksAsync();
}
