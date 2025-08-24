namespace NioxVF.Domain.Models;

/// <summary>
/// Representa un tipo de impuesto aplicado en una factura
/// </summary>
public class TaxItem
{
    /// <summary>
    /// Base imponible
    /// </summary>
    public required decimal TaxBase { get; set; }
    
    /// <summary>
    /// Tipo de gravamen (porcentaje)
    /// </summary>
    public required decimal TaxRate { get; set; }
    
    /// <summary>
    /// Cuota del impuesto
    /// </summary>
    public required decimal TaxAmount { get; set; }
    
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
