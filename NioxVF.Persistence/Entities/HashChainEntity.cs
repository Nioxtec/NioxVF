namespace NioxVF.Persistence.Entities;

/// <summary>
/// Entidad para persistir la cadena de hashes por vendedor y serie
/// </summary>
public class HashChainEntity : Base.AuditEntity
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
    /// Hash actual de la cadena
    /// </summary>
    public string CurrentHash { get; set; } = string.Empty;

    /// <summary>
    /// Hash anterior en la cadena
    /// </summary>
    public string? PreviousHash { get; set; }

    /// <summary>
    /// Fecha y hora de la última actualización de la cadena
    /// </summary>
    public DateTime LastUpdated { get; set; }
}
