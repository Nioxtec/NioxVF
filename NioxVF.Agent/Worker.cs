using NioxVF.Agent.Services;
using NioxVF.Domain.Interfaces;

namespace NioxVF.Agent;

/// <summary>
/// Servicio principal del Agent que procesa tickets de Aronium
/// </summary>
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private readonly IInboxReader _inboxReader;
    private readonly IApiClient _apiClient;
    private readonly IQrGenerator _qrGenerator;
    private readonly int _intervalSeconds;

    public Worker(
        ILogger<Worker> logger, 
        IConfiguration configuration,
        IInboxReader inboxReader,
        IApiClient apiClient,
        IQrGenerator qrGenerator)
    {
        _logger = logger;
        _configuration = configuration;
        _inboxReader = inboxReader;
        _apiClient = apiClient;
        _qrGenerator = qrGenerator;
        _intervalSeconds = _configuration.GetValue<int>("IntervalSeconds", 5);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("NioxVF Agent started. Polling interval: {IntervalSeconds}s", _intervalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingTickets(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in main processing loop");
            }

            await Task.Delay(TimeSpan.FromSeconds(_intervalSeconds), stoppingToken);
        }

        _logger.LogInformation("NioxVF Agent stopped");
    }

    private async Task ProcessPendingTickets(CancellationToken cancellationToken)
    {
        var tickets = await _inboxReader.ReadPendingTicketsAsync();
        
        if (!tickets.Any())
        {
            _logger.LogDebug("No pending tickets found");
            return;
        }

        _logger.LogInformation("Found {Count} pending tickets", tickets.Count);

        foreach (var ticket in tickets)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                await ProcessSingleTicket(ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing ticket {TicketId}", ticket.Id);
            }
        }
    }

    private async Task ProcessSingleTicket(TicketInfo ticket)
    {
        _logger.LogInformation("Processing ticket {Series}-{Number} from {SellerNif}", 
            ticket.Invoice.Series, ticket.Invoice.Number, ticket.Invoice.SellerNif);

        try
        {
            // Enviar a la API
            var mode = _configuration.GetValue<string>("SignMode", "sign-and-send") ?? "sign-and-send";
            var response = await _apiClient.ProcessInvoiceAsync(ticket.Invoice, mode);

            if (response != null)
            {
                _logger.LogInformation("Ticket processed successfully. ID: {Id}, Status: {Status}, AEAT ID: {AeatId}", 
                    response.Id, response.Status, response.AeatId);

                // Generar QR si hay respuesta exitosa
                if (!string.IsNullOrEmpty(response.AeatId))
                {
                    await GenerateQrForTicket(ticket, response);
                }

                // Marcar como procesado
                await _inboxReader.MarkTicketAsProcessedAsync(ticket.Id);
            }
            else
            {
                _logger.LogWarning("No response received for ticket {TicketId}", ticket.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process ticket {TicketId}", ticket.Id);
            await _inboxReader.MarkTicketAsFailedAsync(ticket.Id, ex.Message);
        }
    }

    private async Task GenerateQrForTicket(TicketInfo ticket, InvoiceResponse response)
    {
        try
        {
            var qrPath = _configuration.GetValue<string>("QrPath", @"C:\NioxVF\qr") ?? @"C:\NioxVF\qr";
            var fileName = $"{ticket.Invoice.Series}{ticket.Invoice.Number}.png";
            var fullPath = Path.Combine(qrPath, fileName);

            // Asegurar que el directorio existe
            Directory.CreateDirectory(qrPath);

            // Generar QR con la URL de validación
            var qrData = response.ValidationUrl ?? $"https://sede.agenciatributaria.gob.es/ValidarQR?id={response.AeatId}";
            await _qrGenerator.GenerateQrAsync(qrData, fullPath);

            _logger.LogInformation("QR generated for ticket {Series}-{Number} at {Path}", 
                ticket.Invoice.Series, ticket.Invoice.Number, fullPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate QR for ticket {TicketId}", ticket.Id);
        }
    }
}
