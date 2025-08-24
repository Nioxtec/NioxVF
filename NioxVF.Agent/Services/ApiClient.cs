using System.Text;
using System.Text.Json;
using NioxVF.Domain.Models;

namespace NioxVF.Agent.Services;

/// <summary>
/// Cliente HTTP para comunicación con la API NioxVF
/// </summary>
public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiClient> _logger;
    private readonly string _apiKey;

    public ApiClient(HttpClient httpClient, ILogger<ApiClient> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = configuration.GetValue<string>("ApiKey") ?? throw new InvalidOperationException("ApiKey not configured");
        
        var apiBaseUrl = configuration.GetValue<string>("ApiBaseUrl") ?? "http://localhost:5180";
        _httpClient.BaseAddress = new Uri(apiBaseUrl);
        _httpClient.DefaultRequestHeaders.Add("X-API-Key", _apiKey);
    }

    public async Task<InvoiceResponse?> ProcessInvoiceAsync(InvoiceSimple invoice, string mode, string? xmlSignedBase64 = null)
    {
        try
        {
            var request = new
            {
                invoice = invoice,
                mode = mode,
                xmlSignedBase64 = xmlSignedBase64
            };

            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogDebug("Sending invoice {Series}-{Number} to API", invoice.Series, invoice.Number);

            var response = await _httpClient.PostAsync("/api/v1/invoices", content);

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var invoiceResponse = JsonSerializer.Deserialize<InvoiceResponse>(responseJson, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                _logger.LogInformation("Invoice {Series}-{Number} processed successfully by API", invoice.Series, invoice.Number);
                return invoiceResponse;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("API error {StatusCode}: {Content}", response.StatusCode, errorContent);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling API for invoice {Series}-{Number}", invoice.Series, invoice.Number);
            return null;
        }
    }

    public async Task<InvoiceResponse?> GetInvoiceStatusAsync(string invoiceId)
    {
        try
        {
            _logger.LogDebug("Getting status for invoice {InvoiceId}", invoiceId);

            var response = await _httpClient.GetAsync($"/api/v1/invoices/{invoiceId}");

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var invoiceResponse = JsonSerializer.Deserialize<InvoiceResponse>(responseJson, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                return invoiceResponse;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Invoice {InvoiceId} not found", invoiceId);
                return null;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("API error {StatusCode}: {Content}", response.StatusCode, errorContent);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting status for invoice {InvoiceId}", invoiceId);
            return null;
        }
    }
}
