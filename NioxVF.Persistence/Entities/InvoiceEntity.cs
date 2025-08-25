namespace NioxVF.Persistence.Entities;

/// <summary>
/// Entidad para persistir las facturas en la base de datos
/// </summary>
public class InvoiceEntity : Base.AuditEntity
{
    /// <summary>
    /// NIF del vendedor
    /// </summary>
    public string SellerNif { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del vendedor
    /// </summary>
    public string SellerName { get; set; } = string.Empty;

    /// <summary>
    /// Serie de la factura
    /// </summary>
    public string Series { get; set; } = string.Empty;

    /// <summary>
    /// Número de la factura
    /// </summary>
    public string Number { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de emisión de la factura
    /// </summary>
    public DateTime IssueDate { get; set; }

    /// <summary>
    /// Texto adicional de la factura (opcional)
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Tipo de factura (por defecto F1)
    /// </summary>
    public string Type { get; set; } = "F1";

    /// <summary>
    /// Hash del documento anterior en la cadena
    /// </summary>
    public string? PrevHash { get; set; }

    /// <summary>
    /// Hash de este documento
    /// </summary>
    public string? Hash { get; set; }

    /// <summary>
    /// Total de bases imponibles (calculado)
    /// </summary>
    public decimal TotalTaxBase { get; set; }

    /// <summary>
    /// Total de impuestos (calculado)
    /// </summary>
    public decimal TotalTaxAmount { get; set; }

    /// <summary>
    /// Total de recargos de equivalencia (calculado)
    /// </summary>
    public decimal TotalSurcharge { get; set; }

    /// <summary>
    /// Total general de la factura (calculado)
    /// </summary>
    public decimal Total { get; set; }

    /// <summary>
    /// Navegación a los elementos de impuestos de esta factura
    /// </summary>
    public List<TaxItemEntity> Taxes { get; set; } = new();
}
