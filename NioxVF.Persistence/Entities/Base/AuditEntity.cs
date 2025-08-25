namespace NioxVF.Persistence.Entities.Base;

/// <summary>
/// Entidad base que proporciona propiedades de auditoría para todas las entidades
/// </summary>
public abstract class AuditEntity
{
    /// <summary>
    /// Identificador único de la entidad
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Fecha y hora de creación de la entidad
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Fecha y hora de la última actualización de la entidad
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Usuario o sistema que creó la entidad
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Usuario o sistema que realizó la última actualización
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Indica si la entidad ha sido eliminada (soft delete)
    /// </summary>
    public bool IsDeleted { get; set; }
}
