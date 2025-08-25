using NioxVF.Persistence.Entities;

namespace NioxVF.Persistence.Interfaces;

/// <summary>
/// Interfaz para el repositorio de facturas con métodos específicos del dominio
/// </summary>
public interface IInvoiceRepository : IRepository<InvoiceEntity>
{
    /// <summary>
    /// Obtiene una factura por su serie y número para un vendedor específico
    /// </summary>
    /// <param name="sellerNif">NIF del vendedor</param>
    /// <param name="series">Serie de la factura</param>
    /// <param name="number">Número de la factura</param>
    /// <returns>La factura encontrada o null si no existe</returns>
    Task<InvoiceEntity?> GetBySeriesAndNumberAsync(string sellerNif, string series, string number);

    /// <summary>
    /// Obtiene todas las facturas de un vendedor específico
    /// </summary>
    /// <param name="sellerNif">NIF del vendedor</param>
    /// <returns>Colección de facturas del vendedor</returns>
    Task<IEnumerable<InvoiceEntity>> GetBySellerAsync(string sellerNif);

    /// <summary>
    /// Obtiene facturas dentro de un rango de fechas
    /// </summary>
    /// <param name="startDate">Fecha de inicio del rango</param>
    /// <param name="endDate">Fecha de fin del rango</param>
    /// <returns>Colección de facturas en el rango de fechas</returns>
    Task<IEnumerable<InvoiceEntity>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Obtiene el último número de factura en una serie específica para un vendedor
    /// </summary>
    /// <param name="sellerNif">NIF del vendedor</param>
    /// <param name="series">Serie de facturación</param>
    /// <returns>El último número de factura o null si no hay facturas en la serie</returns>
    Task<string?> GetLastNumberInSeriesAsync(string sellerNif, string series);
}
