namespace NioxVF.Persistence.Entities;

/// <summary>
/// Entidad para persistir los elementos de impuestos de una factura
/// </summary>
public class TaxItemEntity : Base.AuditEntity
{
    /// <summary>
    /// Identificador de la factura a la que pertenece este elemento de impuesto
    /// </summary>
    public int InvoiceId { get; set; }

    /// <summary>
    /// Navegación a la factura padre
    /// </summary>
    public InvoiceEntity Invoice { get; set; } = null!;

    /// <summary>
    /// Base imponible
    /// </summary>
    public decimal TaxBase { get; set; }

    /// <summary>
    /// Tipo de gravamen (porcentaje)
    /// </summary>
    public decimal TaxRate { get; set; }

    /// <summary>
    /// Cuota del impuesto
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// Tipo de recargo de equivalencia (opcional)
    /// </summary>
    public decimal? SurchargeRate { get; set; }

    /// <summary>
    /// Cuota del recargo de equivalencia (opcional)
    /// </summary>
    public decimal? SurchargeAmount { get; set; }

    /// <summary>
    /// Tipo de impuesto (IVA, IGIC, IPSI)
    /// </summary>
    public string TaxType { get; set; } = "IVA";
}
