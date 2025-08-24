namespace NioxVF.Domain.Models;

/// <summary>
/// Representa una factura simplificada (F1) según el sistema Veri*Factu
/// </summary>
public class InvoiceSimple
{
    public required string SellerNif { get; set; }
    public required string SellerName { get; set; }
    public required string Series { get; set; }
    public required string Number { get; set; }
    public required DateTime IssueDate { get; set; }
    public required List<TaxItem> Taxes { get; set; } = new();
    public string? Text { get; set; }
    public string Type { get; set; } = "F1";
    
    /// <summary>
    /// Hash del documento anterior en la cadena (calculado)
    /// </summary>
    public string? PrevHash { get; set; }
    
    /// <summary>
    /// Hash de este documento (calculado)
    /// </summary>
    public string? Hash { get; set; }
    
    /// <summary>
    /// Calcula el total de bases imponibles
    /// </summary>
    public decimal TotalTaxBase => Taxes.Sum(t => t.TaxBase);
    
    /// <summary>
    /// Calcula el total de impuestos
    /// </summary>
    public decimal TotalTaxAmount => Taxes.Sum(t => t.TaxAmount);
    
    /// <summary>
    /// Calcula el total de recargos de equivalencia
    /// </summary>
    public decimal TotalSurcharge => Taxes.Sum(t => t.SurchargeAmount ?? 0);
    
    /// <summary>
    /// Calcula el total general de la factura
    /// </summary>
    public decimal Total => TotalTaxBase + TotalTaxAmount + TotalSurcharge;
}
