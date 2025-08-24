using NioxVF.Domain.Models;

namespace NioxVF.Agent.Services;

/// <summary>
/// Información de un ticket pendiente de procesar
/// </summary>
public class TicketInfo
{
    public required string Id { get; set; }
    public required InvoiceSimple Invoice { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// Interfaz para leer tickets desde el sistema de Aronium
/// </summary>
public interface IInboxReader
{
    /// <summary>
    /// Lee tickets pendientes de procesar
    /// </summary>
    /// <returns>Lista de tickets pendientes</returns>
    Task<List<TicketInfo>> ReadPendingTicketsAsync();
    
    /// <summary>
    /// Marca un ticket como procesado
    /// </summary>
    /// <param name="ticketId">ID del ticket</param>
    Task MarkTicketAsProcessedAsync(string ticketId);
    
    /// <summary>
    /// Marca un ticket como fallido
    /// </summary>
    /// <param name="ticketId">ID del ticket</param>
    /// <param name="error">Descripción del error</param>
    Task MarkTicketAsFailedAsync(string ticketId, string error);
}
