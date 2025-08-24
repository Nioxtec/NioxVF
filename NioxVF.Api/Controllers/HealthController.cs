using Microsoft.AspNetCore.Mvc;

namespace NioxVF.Api.Controllers;

/// <summary>
/// Controlador para health checks y métricas
/// </summary>
[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Health check básico
    /// </summary>
    /// <returns>Estado de salud del servicio</returns>
    [HttpGet]
    public IActionResult GetHealth()
    {
        var health = new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0",
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
        };

        _logger.LogDebug("Health check requested");
        return Ok(health);
    }

    /// <summary>
    /// Métricas básicas del sistema
    /// </summary>
    /// <returns>Métricas del servicio</returns>
    [HttpGet("metrics")]
    public IActionResult GetMetrics()
    {
        var metrics = new
        {
            Timestamp = DateTime.UtcNow,
            Memory = new
            {
                WorkingSet = GC.GetTotalMemory(false),
                GCCollections = new
                {
                    Gen0 = GC.CollectionCount(0),
                    Gen1 = GC.CollectionCount(1),
                    Gen2 = GC.CollectionCount(2)
                }
            },
            Process = new
            {
                ProcessorTime = Environment.TickCount,
                ThreadCount = Environment.ProcessorCount
            }
        };

        return Ok(metrics);
    }
}
