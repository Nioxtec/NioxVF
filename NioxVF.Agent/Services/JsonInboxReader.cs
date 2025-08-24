using System.Text.Json;
using NioxVF.Domain.Models;

namespace NioxVF.Agent.Services;

/// <summary>
/// Implementación del lector de inbox que lee archivos JSON
/// </summary>
public class JsonInboxReader : IInboxReader
{
    private readonly ILogger<JsonInboxReader> _logger;
    private readonly string _inboxPath;
    private readonly string _processedPath;
    private readonly string _failedPath;

    public JsonInboxReader(ILogger<JsonInboxReader> logger, IConfiguration configuration)
    {
        _logger = logger;
        _inboxPath = configuration.GetValue<string>("InboxPath", @"C:\NioxVF\inbox") ?? @"C:\NioxVF\inbox";
        _processedPath = Path.Combine(_inboxPath, "processed");
        _failedPath = Path.Combine(_inboxPath, "failed");
        
        // Crear directorios si no existen
        Directory.CreateDirectory(_inboxPath);
        Directory.CreateDirectory(_processedPath);
        Directory.CreateDirectory(_failedPath);
    }

    public async Task<List<TicketInfo>> ReadPendingTicketsAsync()
    {
        var tickets = new List<TicketInfo>();

        try
        {
            var jsonFiles = Directory.GetFiles(_inboxPath, "*.json", SearchOption.TopDirectoryOnly);
            
            foreach (var filePath in jsonFiles)
            {
                try
                {
                    var ticket = await ReadTicketFromFileAsync(filePath);
                    if (ticket != null)
                    {
                        tickets.Add(ticket);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error reading ticket from file {FilePath}", filePath);
                    // Mover archivo problemático a carpeta de fallos
                    await MoveToFailedAsync(filePath, ex.Message);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scanning inbox directory {InboxPath}", _inboxPath);
        }

        return tickets;
    }

    public async Task MarkTicketAsProcessedAsync(string ticketId)
    {
        try
        {
            var sourceFile = Path.Combine(_inboxPath, $"{ticketId}.json");
            var targetFile = Path.Combine(_processedPath, $"{ticketId}_{DateTime.UtcNow:yyyyMMddHHmmss}.json");
            
            if (File.Exists(sourceFile))
            {
                File.Move(sourceFile, targetFile);
                _logger.LogDebug("Moved processed ticket {TicketId} to {TargetFile}", ticketId, targetFile);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking ticket {TicketId} as processed", ticketId);
        }
        
        await Task.CompletedTask;
    }

    public async Task MarkTicketAsFailedAsync(string ticketId, string error)
    {
        try
        {
            var sourceFile = Path.Combine(_inboxPath, $"{ticketId}.json");
            var targetFile = Path.Combine(_failedPath, $"{ticketId}_{DateTime.UtcNow:yyyyMMddHHmmss}.json");
            
            if (File.Exists(sourceFile))
            {
                // Agregar información de error al archivo
                var content = await File.ReadAllTextAsync(sourceFile);
                var errorInfo = new { OriginalContent = content, Error = error, Timestamp = DateTime.UtcNow };
                var errorJson = JsonSerializer.Serialize(errorInfo, new JsonSerializerOptions { WriteIndented = true });
                
                await File.WriteAllTextAsync(targetFile, errorJson);
                File.Delete(sourceFile);
                
                _logger.LogDebug("Moved failed ticket {TicketId} to {TargetFile}", ticketId, targetFile);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking ticket {TicketId} as failed", ticketId);
        }
    }

    private async Task<TicketInfo?> ReadTicketFromFileAsync(string filePath)
    {
        var content = await File.ReadAllTextAsync(filePath);
        var fileName = Path.GetFileNameWithoutExtension(filePath);
        
        // Intentar deserializar como InvoiceSimple directamente
        try
        {
            var invoice = JsonSerializer.Deserialize<InvoiceSimple>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (invoice != null)
            {
                return new TicketInfo
                {
                    Id = fileName,
                    Invoice = invoice,
                    CreatedAt = File.GetCreationTime(filePath),
                    Status = "Pending"
                };
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Invalid JSON format in file {FilePath}", filePath);
        }

        return null;
    }

    private async Task MoveToFailedAsync(string filePath, string error)
    {
        try
        {
            var fileName = Path.GetFileName(filePath);
            var targetPath = Path.Combine(_failedPath, $"error_{DateTime.UtcNow:yyyyMMddHHmmss}_{fileName}");
            
            var content = await File.ReadAllTextAsync(filePath);
            var errorInfo = new { OriginalContent = content, Error = error, Timestamp = DateTime.UtcNow };
            var errorJson = JsonSerializer.Serialize(errorInfo, new JsonSerializerOptions { WriteIndented = true });
            
            await File.WriteAllTextAsync(targetPath, errorJson);
            File.Delete(filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving failed file {FilePath}", filePath);
        }
    }
}
