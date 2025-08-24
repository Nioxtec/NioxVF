using System.Security.Cryptography;
using System.Text;
using NioxVF.Domain.Interfaces;
using NioxVF.Domain.Models;

namespace NioxVF.Domain.Services;

/// <summary>
/// Implementación básica del servicio de encadenado de documentos
/// </summary>
public class HashChainService : IHashChain
{
    private readonly Dictionary<string, string> _chainStorage = new();
    
    public Task<string?> GetPreviousHashAsync(string sellerNif, string series)
    {
        var key = $"{sellerNif}|{series}";
        _chainStorage.TryGetValue(key, out var prevHash);
        return Task.FromResult(prevHash);
    }
    
    public string CalculateHash(InvoiceSimple invoice)
    {
        // Payload hash según especificación: SellerNif|Series|Number|IssueDate(yyyyMMdd)|Total|PrevHash
        var dateString = invoice.IssueDate.ToString("yyyyMMdd");
        var payload = $"{invoice.SellerNif}|{invoice.Series}|{invoice.Number}|{dateString}|{invoice.Total:F2}|{invoice.PrevHash ?? ""}";
        
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(hashBytes);
    }
    
    public Task UpdateChainAsync(string sellerNif, string series, string? prevHash, string currentHash)
    {
        var key = $"{sellerNif}|{series}";
        _chainStorage[key] = currentHash;
        return Task.CompletedTask;
    }
}
