namespace NioxVF.Persistence.Entities;

/// <summary>
/// Entidad para el control de concurrencia por serie y vendedor
/// </summary>
public class SeriesLockEntity : Base.AuditEntity
{
    /// <summary>
    /// NIF del vendedor
    /// </summary>
    public string SellerNif { get; set; } = string.Empty;

    /// <summary>
    /// Serie de facturación
    /// </summary>
    public string Series { get; set; } = string.Empty;

    /// <summary>
    /// Identificador único del lock
    /// </summary>
    public string LockId { get; set; } = string.Empty;

    /// <summary>
    /// Fecha y hora en que se adquirió el lock
    /// </summary>
    public DateTime AcquiredAt { get; set; }

    /// <summary>
    /// Fecha y hora en que expira el lock
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Indica si el lock está activo
    /// </summary>
    public bool IsActive { get; set; } = true;
}
